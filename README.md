# G360 Orders

**Ordering Management System** — Clean Architecture .NET 9 API with REST (FastEndpoints), GraphQL (HotChocolate), and SQLite.

---

## Features

- **REST API** (FastEndpoints) — kebab-case routes under `/api` (orders, ingredients, categories, pizzas, pizza-types)
- **GraphQL** (HotChocolate) — queries and mutations at `/graphql` with Banana Cake Pop
- **Scalar** — OpenAPI-based REST documentation at `/scalar/v1`
- **Audit** — `X-User-Id` header; automatic CreatedBy/UpdatedBy and soft delete via EF interceptor
- **Clean Architecture** — Domain → Application → Infrastructure → Presentation
- **CQRS** — MediatR commands and queries with FluentValidation and AutoMapper
- **SQLite + EF Core** — migrations, design-time factory for `dotnet ef`
- **xUnit** — unit tests for Domain, Application, Infrastructure

---

## Solution structure

| Project | Description |
|--------|-------------|
| **G360.Orders.Domain** | Entities (Order, OrderDetail, Pizza, PizzaType, PizzaDetail, Ingredient, Category), interfaces (IEntity, IAuditableEntity, IRepository, IOrderRepository, IPizzaDetailRepository, IAuditUserProvider) |
| **G360.Orders.Application** | CQRS (MediatR), commands/queries, handlers, validators (FluentValidation), Response/PagedResponse, AutoMapper profiles |
| **G360.Orders.Infrastructure** | EF Core SQLite (OrdersDbContext), repositories, AuditSaveChangesInterceptor, design-time DbContext factory |
| **G360.Orders.Presentation.WebApi** | FastEndpoints, GraphQL, Scalar, AuditUserMiddleware |

---

## Quick start

```bash
# Run the API
dotnet run --project src/presentation/G360.Orders.Presentation.WebApi/G360.Orders.Presentation.WebApi.csproj
```

| URL | Purpose |
|-----|---------|
| `https://localhost:<port>/api/orders`, `/api/ingredients`, `/api/categories`, `/api/pizzas`, `/api/pizza-types` | REST API |
| `https://localhost:<port>/scalar/v1` | REST docs (Scalar) |
| `https://localhost:<port>/graphql` | GraphQL (Banana Cake Pop) |

**Header:** Send `X-User-Id: your-user-id` for requests that create or update data.

```bash
# Run tests
dotnet test G360.Orders.sln
```

---