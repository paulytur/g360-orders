namespace G360.Orders.Application.Services;

/// <summary>
/// Imports CSV data from a folder into the database.
/// </summary>
public interface IDataImportService
{
    /// <summary>
    /// Imports all CSV files from the given folder (pizza_types.csv, pizzas.csv, orders.csv, order_details.csv)
    /// into the database in the correct order. Returns counts per entity type.
    /// </summary>
    Task<DataImportResult> ImportFromFolderAsync(string dataFolderPath, CancellationToken cancellationToken = default);
}

public class DataImportResult
{
    public int CategoriesCreated { get; set; }
    public int IngredientsCreated { get; set; }
    public int PizzaTypesCreated { get; set; }
    public int PizzaDetailsCreated { get; set; }
    public int PizzasCreated { get; set; }
    public int OrdersCreated { get; set; }
    public int OrderDetailsCreated { get; set; }
    public IList<string> Errors { get; set; } = new List<string>();
    public bool Success => Errors.Count == 0;
}
