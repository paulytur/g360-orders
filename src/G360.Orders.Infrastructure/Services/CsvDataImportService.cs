using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using G360.Orders.Application.Services;
using G360.Orders.Domain.Entities;
using G360.Orders.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace G360.Orders.Infrastructure.Services;

public class CsvDataImportService : IDataImportService
{
    private const string AuditUser = "import";
    private static readonly DateTime AuditNow = DateTime.UtcNow;

    private readonly OrdersDbContext _db;

    public CsvDataImportService(OrdersDbContext db)
    {
        _db = db;
    }

    public async Task<DataImportResult> ImportFromFolderAsync(string dataFolderPath, CancellationToken cancellationToken = default)
    {
        var result = new DataImportResult();
        if (!Directory.Exists(dataFolderPath))
        {
            result.Errors.Add($"Data folder not found: {dataFolderPath}");
            return result;
        }

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null
        };

        try
        {
            // Big speedups for bulk loads
            var previousDetectChanges = _db.ChangeTracker.AutoDetectChangesEnabled;
            _db.ChangeTracker.AutoDetectChangesEnabled = false;

            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
            // 1. Pizza types CSV -> Categories, Ingredients, PizzaTypes, PizzaDetails
            var pizzaTypesPath = Path.Combine(dataFolderPath, "pizza_types.csv");
            if (!File.Exists(pizzaTypesPath))
            {
                result.Errors.Add($"File not found: {pizzaTypesPath}");
                return result;
            }

            // Preload existing (so re-running import doesn't blow up immediately; still may create duplicates for PizzaTypes/Pizzas/Orders)
            var categoryByName = await _db.Categories.AsNoTracking()
                .ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase, cancellationToken);
            var ingredientByDesc = await _db.Ingredients.AsNoTracking()
                .ToDictionaryAsync(i => i.Description, StringComparer.OrdinalIgnoreCase, cancellationToken);
            var pizzaTypeByCode = await _db.PizzaTypes.AsNoTracking()
                .ToDictionaryAsync(p => p.Code, StringComparer.OrdinalIgnoreCase, cancellationToken);

            // Stage from CSV (so we can insert in bulk and still create junction rows)
            var pizzaTypeRows = new List<PizzaTypeCsvRow>(256);
            var neededCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var neededIngredients = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var reader = new StreamReader(pizzaTypesPath))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                var records = csv.GetRecordsAsync<PizzaTypeCsvRow>(cancellationToken);
                await foreach (var row in records)
                {
                    if (string.IsNullOrWhiteSpace(row.pizza_type_id)) continue;
                    pizzaTypeRows.Add(row);
                    if (!string.IsNullOrWhiteSpace(row.category))
                        neededCategories.Add(row.category.Trim());

                    foreach (var ing in (row.ingredients ?? "").Split(',', StringSplitOptions.TrimEntries))
                        if (!string.IsNullOrWhiteSpace(ing))
                            neededIngredients.Add(ing.Trim());
                }
            }

            // Insert missing Categories
            var newCategories = new List<Category>();
            foreach (var name in neededCategories)
            {
                if (categoryByName.ContainsKey(name)) continue;
                newCategories.Add(new Category
                {
                    Name = name,
                    IsDeleted = false,
                    CreatedBy = AuditUser,
                    CreatedDatetime = AuditNow,
                    UpdatedBy = AuditUser,
                    UpdatedDatetime = AuditNow
                });
            }
            if (newCategories.Count > 0)
            {
                _db.Categories.AddRange(newCategories);
                await _db.SaveChangesAsync(cancellationToken);
                result.CategoriesCreated += newCategories.Count;
                foreach (var c in newCategories)
                    categoryByName[c.Name] = c;
            }

            // Insert missing Ingredients
            
            var newIngredients = new List<Ingredient>();
            foreach (var desc in neededIngredients)
            {
                if (ingredientByDesc.ContainsKey(desc)) continue;
                newIngredients.Add(new Ingredient
                {
                    Description = desc,
                    IsDeleted = false,
                    CreatedBy = AuditUser,
                    CreatedDatetime = AuditNow,
                    UpdatedBy = AuditUser,
                    UpdatedDatetime = AuditNow
                });
            }
            
            if (newIngredients.Count > 0)
            {
                _db.Ingredients.AddRange(newIngredients);
                await _db.SaveChangesAsync(cancellationToken);
                result.IngredientsCreated += newIngredients.Count;
                foreach (var i in newIngredients)
                    ingredientByDesc[i.Description] = i;
            }

            // Insert PizzaTypes (skip existing codes)
            var newPizzaTypes = new List<PizzaType>();
            foreach (var row in pizzaTypeRows)
            {
                var code = row.pizza_type_id!.Trim();
                if (pizzaTypeByCode.ContainsKey(code)) continue;

                long? categoryId = null;
                if (!string.IsNullOrWhiteSpace(row.category) && categoryByName.TryGetValue(row.category.Trim(), out var cat))
                    categoryId = cat.Id;

                newPizzaTypes.Add(new PizzaType
                {
                    Code = code,
                    Name = (row.name ?? row.pizza_type_id).Trim(),
                    CategoryId = categoryId,
                    IsDeleted = false,
                    CreatedBy = AuditUser,
                    CreatedDatetime = AuditNow,
                    UpdatedBy = AuditUser,
                    UpdatedDatetime = AuditNow
                });
            }
            
            if (newPizzaTypes.Count > 0)
            {
                _db.PizzaTypes.AddRange(newPizzaTypes);
                await _db.SaveChangesAsync(cancellationToken);
                result.PizzaTypesCreated += newPizzaTypes.Count;
                foreach (var pt in newPizzaTypes)
                    pizzaTypeByCode[pt.Code] = pt;
            }

            // Insert PizzaDetails junction rows in batches, avoiding duplicates (in DB + in this import run)
            // Preload existing keys so re-running import won't violate the unique constraint.
            var detailKeys = await _db.PizzaDetails.AsNoTracking()
                .Select(d => new ValueTuple<long, long>(d.PizzaTypeId, d.IngredientId))
                .ToHashSetAsync(cancellationToken);
            
            var detailsBatch = new List<PizzaDetail>(5000);
            
            foreach (var row in pizzaTypeRows)
            {
                var code = row.pizza_type_id!.Trim();
                
                if (!pizzaTypeByCode.TryGetValue(code, out var pt)) continue;

                foreach (var ing in (row.ingredients ?? "").Split(',', StringSplitOptions.TrimEntries))
                {
                    var desc = ing.Trim();
                    if (string.IsNullOrWhiteSpace(desc)) continue;
                    if (!ingredientByDesc.TryGetValue(desc, out var ingredient)) continue;

                    var key = (pt.Id, ingredient.Id);
                    
                    if (!detailKeys.Add(key)) continue;

                    detailsBatch.Add(new PizzaDetail
                    {
                        PizzaTypeId = pt.Id,
                        IngredientId = ingredient.Id,
                        IsDeleted = false,
                        CreatedBy = AuditUser,
                        CreatedDatetime = AuditNow,
                        UpdatedBy = AuditUser,
                        UpdatedDatetime = AuditNow
                    });
                    
                    if (detailsBatch.Count >= 5000)
                    {
                        _db.PizzaDetails.AddRange(detailsBatch);
                        await _db.SaveChangesAsync(cancellationToken);
                        result.PizzaDetailsCreated += detailsBatch.Count;
                        detailsBatch.Clear();
                    }
                }
            }
            
            if (detailsBatch.Count > 0)
            {
                _db.PizzaDetails.AddRange(detailsBatch);
                await _db.SaveChangesAsync(cancellationToken);
                result.PizzaDetailsCreated += detailsBatch.Count;
            }

            // 2. Pizzas CSV
            var pizzasPath = Path.Combine(dataFolderPath, "pizzas.csv");
            if (!File.Exists(pizzasPath))
            {
                result.Errors.Add($"File not found: {pizzasPath}");
                return result;
            }

            var pizzaByCode = await _db.Pizzas.AsNoTracking()
                .ToDictionaryAsync(p => p.Code, StringComparer.OrdinalIgnoreCase, cancellationToken);
            var newPizzas = new List<Pizza>(1024);
            using (var reader = new StreamReader(pizzasPath))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                var records = csv.GetRecordsAsync<PizzaCsvRow>(cancellationToken);
                await foreach (var row in records)
                {
                    if (string.IsNullOrWhiteSpace(row.pizza_id)) continue;
                    var code = row.pizza_id.Trim();
                    if (pizzaByCode.ContainsKey(code)) continue;
                    if (!pizzaTypeByCode.TryGetValue(row.pizza_type_id ?? "", out var pt))
                        continue;

                    newPizzas.Add(new Pizza
                    {
                        Code = code,
                        Size = row.size?.Trim(),
                        Price = decimal.TryParse(row.price, NumberStyles.Any, CultureInfo.InvariantCulture, out var p) ? p : 0,
                        PizzaTypeId = pt.Id,
                        IsDeleted = false,
                        CreatedBy = AuditUser,
                        CreatedDatetime = AuditNow,
                        UpdatedBy = AuditUser,
                        UpdatedDatetime = AuditNow
                    });
                }
            }

            if (newPizzas.Count > 0)
            {
                _db.Pizzas.AddRange(newPizzas);
                await _db.SaveChangesAsync(cancellationToken);
                result.PizzasCreated += newPizzas.Count;
                foreach (var pz in newPizzas)
                    pizzaByCode[pz.Code] = pz;
            }

            // 3. Orders CSV
            var ordersPath = Path.Combine(dataFolderPath, "orders.csv");
            if (!File.Exists(ordersPath))
            {
                result.Errors.Add($"File not found: {ordersPath}");
                return result;
            }

            // Fast path: in SQLite you can insert explicit values into INTEGER PRIMARY KEY.
            // We insert Orders with Id = CSV order_id so order_details can reference without mapping.
            var existingOrderIds = await _db.Orders.AsNoTracking()
                .Select(o => o.Id)
                .ToHashSetAsync(cancellationToken);
            using (var reader = new StreamReader(ordersPath))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                var records = csv.GetRecordsAsync<OrderCsvRow>(cancellationToken);
                await foreach (var row in records)
                {
                    if (!long.TryParse(row.order_id, out var csvOrderId)) continue;
                    if (!existingOrderIds.Add(csvOrderId)) continue; // already in DB
                    var date = DateTime.TryParse(row.date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) ? d : AuditNow;
                    var time = TimeOnly.TryParse(row.time ?? "0:0:0", out var t) ? t : default;
                    var created = date.Date.Add(t.ToTimeSpan());

                    var order = new Order
                    {
                        Id = csvOrderId,
                        IsDeleted = false,
                        CreatedBy = AuditUser,
                        CreatedDatetime = created,
                        UpdatedBy = AuditUser,
                        UpdatedDatetime = AuditNow
                    };
                    _db.Orders.Add(order);
                    result.OrdersCreated++;
                    if (result.OrdersCreated % 1000 == 0)
                        await _db.SaveChangesAsync(cancellationToken);
                }
            }
            await _db.SaveChangesAsync(cancellationToken);

            // 4. Order details CSV
            var orderDetailsPath = Path.Combine(dataFolderPath, "order_details.csv");
            if (!File.Exists(orderDetailsPath))
            {
                result.Errors.Add($"File not found: {orderDetailsPath}");
                return result;
            }

            var odBatch = new List<OrderDetail>(1000);
            using (var reader = new StreamReader(orderDetailsPath))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                var records = csv.GetRecordsAsync<OrderDetailCsvRow>(cancellationToken);
                await foreach (var row in records)
                {
                    if (!long.TryParse(row.order_id, out var csvOrderId))
                        continue;
                    if (!pizzaByCode.TryGetValue(row.pizza_id ?? "", out var pizza))
                        continue;
                    var qty = int.TryParse(row.quantity, NumberStyles.Integer, CultureInfo.InvariantCulture, out var q) ? q : 1;

                    odBatch.Add(new OrderDetail
                    {
                        OrderId = csvOrderId,
                        PizzaId = pizza.Id,
                        Quantity = qty,
                        IsDeleted = false,
                        CreatedBy = AuditUser,
                        CreatedDatetime = AuditNow,
                        UpdatedBy = AuditUser,
                        UpdatedDatetime = AuditNow
                    });
                    result.OrderDetailsCreated++;
                    if (odBatch.Count >= 1000)
                    {
                        _db.OrderDetails.AddRange(odBatch);
                        await _db.SaveChangesAsync(cancellationToken);
                        odBatch.Clear();
                    }
                }
            }
            if (odBatch.Count > 0)
            {
                _db.OrderDetails.AddRange(odBatch);
                await _db.SaveChangesAsync(cancellationToken);
            }

            await tx.CommitAsync(cancellationToken);
            }
            finally
            {
                _db.ChangeTracker.AutoDetectChangesEnabled = previousDetectChanges;
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add(ex.Message);
            
        }

        return result;
    }

    private class PizzaTypeCsvRow
    {
        public string? pizza_type_id { get; set; }
        public string? name { get; set; }
        public string? category { get; set; }
        public string? ingredients { get; set; }
    }

    private class PizzaCsvRow
    {
        public string? pizza_id { get; set; }
        public string? pizza_type_id { get; set; }
        public string? size { get; set; }
        public string? price { get; set; }
    }

    private class OrderCsvRow
    {
        public string? order_id { get; set; }
        public string? date { get; set; }
        public string? time { get; set; }
    }

    private class OrderDetailCsvRow
    {
        public string? order_details_id { get; set; }
        public string? order_id { get; set; }
        public string? pizza_id { get; set; }
        public string? quantity { get; set; }
    }
}
