using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;
using G360.Orders.Application.Services;
using G360.Orders.Infrastructure.Data;
using G360.Orders.Infrastructure.Interceptors;
using G360.Orders.Infrastructure.Repositories;
using G360.Orders.Infrastructure.Services;
using G360.Orders.Presentation.WebApi.GraphQL;
using G360.Orders.Presentation.WebApi.Helpers;
using G360.Orders.Presentation.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    o.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddScoped<IAuditUserProvider, AuditUserProvider>();
builder.Services.AddScoped<AuditSaveChangesInterceptor>();
// Default: use orders.db in the Infrastructure project folder (resolved from app base so it works from project or bin)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    // From bin/Debug/net9.0 go up to src, then into G360.Orders.Infrastructure
var infrastructureDb = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "G360.Orders.Infrastructure", "orders.db"));
    connectionString = $"Data Source={infrastructureDb}";
}
builder.Services.AddDbContext<OrdersDbContext>((sp, options) =>
{
    options.UseSqlite(connectionString);
    options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
});

builder.Services.AddScoped<IRepository<G360.Orders.Domain.Entities.Order>, OrderRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IRepository<OrderDetail>, OrderDetailRepository>();
builder.Services.AddScoped<IRepository<Pizza>, PizzaRepository>();
builder.Services.AddScoped<IRepository<PizzaType>, PizzaTypeRepository>();
builder.Services.AddScoped<IRepository<PizzaDetail>, PizzaDetailRepository>();
builder.Services.AddScoped<IPizzaDetailRepository, PizzaDetailRepository>();
builder.Services.AddScoped<IRepository<Ingredient>, IngredientRepository>();
builder.Services.AddScoped<IRepository<Category>, CategoryRepository>();
builder.Services.AddScoped<IInsightsRepository, InsightsRepository>();
builder.Services.AddScoped<IDataImportService, CsvDataImportService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(G360.Orders.Application.Queries.GetOrdersQuery).Assembly));
builder.Services.AddAutoMapper(typeof(G360.Orders.Application.Mapping.OrderMappingProfile).Assembly);

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.AutoTagPathSegmentIndex = 0;
        o.DocumentSettings = s =>
        {
            s.Title = "G360 Orders API";
            s.Version = "v1";
            s.Description = "All API requests that create or modify data must include the **X-User-Id** request header with a non-empty value. " +
                "This header is used for audit fields (CreatedBy, UpdatedBy). Requests without it receive **400 Bad Request**. " +
                "Paths under `/swagger`, `/scalar`, `/health`, and `/graphql` do not require the header; they use the default audit user `system`.";
        };
    });

builder.Services.AddGraphQLServer()
    .ModifyOptions(opt => opt.UseXmlDocumentation = true)
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = builder.Environment.IsDevelopment())
    .AddQueryType<OrderQuery>(d => d.Name("Query"))
    .AddTypeExtension<PizzaQuery>()
    .AddTypeExtension<IngredientQuery>()
    .AddTypeExtension<CategoryQuery>()
    .AddTypeExtension<PizzaTypeQuery>()
    .AddTypeExtension<InsightsQuery>()
    .AddMutationType<OrderMutation>(d => d.Name("Mutation"))
    .AddTypeExtension<PizzaMutation>()
    .AddTypeExtension<IngredientMutation>()
    .AddTypeExtension<CategoryMutation>()
    .AddTypeExtension<PizzaTypeMutation>()
    .AddFiltering()
    .AddSorting();

builder.Services.AddCors(o => o.AddDefaultPolicy(policy =>
{
    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

app.UseCors();
app.UseHttpsRedirection();
app.UseMiddleware<AuditUserMiddleware>();

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
})
.UseSwaggerGen();

app.MapScalarApiReference(options =>
{
    options.WithTitle("G360 Orders API - Scalar");
    options.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
});

app.MapGraphQL();
app.Run();
