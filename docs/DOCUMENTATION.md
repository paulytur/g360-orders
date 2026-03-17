# G360 Orders – Technical Documentation

**Ordering Management System** — Clean Architecture .NET 9 API with REST (FastEndpoints), GraphQL (HotChocolate), and SQLite.

---

## 1. Project Overview

G360 Orders is a **pizza ordering management system** that supports:

- **Orders** with line items (pizza + quantity)
- **Pizzas** (code, type, size, price)
- **Pizza types** (e.g. Margherita) with optional **category** and **ingredients**
- **Categories** for grouping pizza types
- **Ingredients** linked to pizza types via a junction table (**PizzaDetail**)

All write operations are **audited** (CreatedBy, UpdatedBy, CreatedDatetime, UpdatedDatetime) and support **soft delete** (IsDeleted). The audit user is supplied via the **X-User-Id** HTTP header.

---

## 2. Architecture

### 2.1 Clean Architecture

The solution follows **Clean Architecture** with clear layer boundaries and dependency direction: **Domain → Application → Infrastructure → Presentation**.

| Layer | Project | Responsibility |
|-------|---------|----------------|
| **Domain** | G360.Orders.Domain | Entities, interfaces (IRepository, IAuditableEntity, IAuditUserProvider). No dependencies on other projects. |
| **Application** | G360.Orders.Application | Use cases: CQRS (MediatR), Commands/Queries, Handlers, Validators (FluentValidation), DTOs (Response, PagedResponse), AutoMapper profiles. Depends only on Domain. |
| **Infrastructure** | G360.Orders.Infrastructure | Persistence: EF Core, OrdersDbContext, repositories, AuditSaveChangesInterceptor, design-time DbContext factory. Depends on Domain and Application. |
| **Presentation** | G360.Orders.Presentation.WebApi | API: FastEndpoints (REST), HotChocolate (GraphQL), Scalar, middleware (audit user), DI wiring. Depends on Application and Infrastructure. |

### 2.2 CQRS (Command Query Responsibility Segregation)

- **Queries** (read): e.g. `GetOrdersQuery`, `GetIngredientByIdQuery` → handlers return `Response<T>` or `PagedResponse<T>`.
- **Commands** (write): e.g. `CreateOrderCommand`, `UpdateCategoryCommand` → handlers perform changes and return `Response` or `Response<T>`.

MediatR dispatches all commands and queries; handlers use repositories and avoid business logic in the API layer.

### 2.3 Repository Pattern

- **Generic**: `IRepository<T>` with `AddAsync`, `UpdateAsync`, `GetByIdAsync`, `GetAll`.
- **Specific**: `IOrderRepository` (order + details), `IPizzaDetailRepository` (junction for PizzaType ↔ Ingredient).

Repositories live in Infrastructure; interfaces in Domain so Application stays persistence-agnostic.

---

## 3. Technology Stack

| Area | Technology |
|------|------------|
| Runtime | .NET 9 |
| REST API | FastEndpoints |
| API docs (REST) | Scalar (OpenAPI) |
| GraphQL | HotChocolate 13 (queries + mutations) |
| ORM | Entity Framework Core 9 |
| Database | SQLite |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Testing | xUnit |

---

## 4. Solution Structure

```
g36_orders/
├── G360.Orders.sln
├── src/
│   ├── G360.Orders.Domain/           # Entities, interfaces
│   ├── G360.Orders.Application/      # CQRS, handlers, validators, mapping
│   ├── G360.Orders.Infrastructure/   # EF Core, repositories, interceptors
│   └── presentation/
│       └── G360.Orders.Presentation.WebApi/   # REST, GraphQL, middleware
├── tests/
│   ├── G360.Orders.Domain.Test/
│   ├── G360.Orders.Application.Test/
│   └── G360.Orders.Infrastructure.Test/
└── docs/
    ├── DOCUMENTATION.md             # This file
    ├── ERD.md                       # Mermaid ERD for the schema
    └── GraphQL-Samples.graphql       # Sample queries/mutations
```

---

## 5. Domain Model

### 5.1 Entities

| Entity | Purpose |
|--------|---------|
| **Order** | Header of an order; has many OrderDetails. |
| **OrderDetail** | Line item: OrderId, PizzaId, Quantity. |
| **Pizza** | Sellable pizza (Code, Type, Size, Price). |
| **PizzaType** | Template (e.g. Margherita); has CategoryId and many PizzaDetails (ingredients). |
| **PizzaDetail** | Junction: PizzaTypeId, IngredientId (many-to-many). |
| **Ingredient** | Description only. |
| **Category** | Name; groups PizzaTypes. |

All entities implement **IEntity** and **IAuditableEntity** (Id, IsDeleted, CreatedBy, CreatedDatetime, UpdatedBy, UpdatedDatetime).

### 5.2 Database Schema (SQLite)

- **Orders** — id, is_deleted, created_by, created_datetime, updated_by, updated_datetime  
- **OrderDetails** — id, order_id, pizza_id, quantity + audit  
- **Pizzas** — id, code, type, size, price + audit  
- **PizzaTypes** — id, code, name, category_id + audit  
- **PizzaDetails** — id, pizza_type_id, ingredient_id + audit (unique on pizza_type_id + ingredient_id)  
- **Ingredients** — id, description + audit  
- **Categories** — id, name + audit  

Migrations are in `G360.Orders.Infrastructure/Data/Migrations`. A design-time factory (`OrdersDbContextFactory`) allows `dotnet ef` to run without the full app host.

---

## 6. REST API (FastEndpoints)

- **Base path**: `/api`
- **Route style**: kebab-case (e.g. `/api/pizza-types`, `/api/orders`).

### 6.1 Endpoints Summary

| Resource | GET (list) | GET (by id) | POST | PUT | DELETE |
|----------|------------|-------------|------|-----|--------|
| Orders | `/api/orders` | `/api/orders/{id}` | `/api/orders` | `/api/orders/{id}` | `/api/orders/{id}` |
| Ingredients | `/api/ingredients` | `/api/ingredients/{id}` | `/api/ingredients` | `/api/ingredients/{id}` | `/api/ingredients/{id}` |
| Categories | `/api/categories` | `/api/categories/{id}` | `/api/categories` | `/api/categories/{id}` | `/api/categories/{id}` |
| Pizzas | `/api/pizzas` | `/api/pizzas/{id}` | `/api/pizzas` | `/api/pizzas/{id}` | `/api/pizzas/{id}` |
| Pizza Types | `/api/pizza-types` | `/api/pizza-types/{id}` | `/api/pizza-types` | `/api/pizza-types/{id}` | `/api/pizza-types/{id}` |

List endpoints support query parameters for paging (`pageNumber`, `pageSize`) and optional filters (e.g. `description`, `code`, `categoryId`). **X-User-Id** is required for all mutating and most read operations (see §7).

### 6.2 Data Import (CSV)

This project includes a convenience endpoint to import the sample CSV files into SQLite using the current schema.

- **Endpoint**: `POST /api/data/import`
- **Default folder**: `docs/Data`
- **Config override**: set `DataImport:Folder` in `appsettings.json` / `appsettings.Development.json`

CSV files expected in the folder:

- `pizza_types.csv` (pizza types + category + ingredients)
- `pizzas.csv` (pizzas linked to pizza_type_id)
- `orders.csv`
- `order_details.csv`

Notes:

- The importer is optimized for bulk loading (batches, transaction, and change-tracking disabled) to keep runtime low for ~50K rows.
- Importing into a database that already contains data may skip some rows (e.g. existing orders/pizza details) and can still create duplicates for tables without a natural unique key beyond `Id`.
- For repeatable “clean loads”, delete `orders.db` first (or add a dedicated truncate/reset flow).

---

## 7. GraphQL (HotChocolate)

- **Endpoint**: `/graphql`
- **Tool**: Banana Cake Pop (built-in) for introspection and testing.

### 7.1 Queries

- `getOrders`, `getOrderById`
- `getIngredients`, `getIngredientById`
- `getCategories`, `getCategoryById`
- `getPizzas`, `getPizzaById`
- `getPizzaTypes`, `getPizzaTypeById`

### 7.2 Mutations

- `createOrder`, `updateOrder`, `deleteOrder`
- `createIngredient`, `updateIngredient`, `deleteIngredient`
- `createCategory`, `updateCategory`, `deleteCategory`
- `createPizza`, `updatePizza`, `deletePizza`
- `createPizzaType`, `updatePizzaType`, `deletePizzaType`

Root types are named **Query** and **Mutation**; extensions (e.g. `IngredientQuery`, `CategoryMutation`) are merged via `[ExtendObjectType(typeof(OrderQuery))]` and `[ExtendObjectType(typeof(OrderMutation))]`. Sample operations are in `docs/GraphQL-Samples.graphql`.

---

## 8. Audit and X-User-Id

### 8.1 Flow

1. **AuditUserMiddleware** runs early in the pipeline. It reads the **X-User-Id** header and calls `IAuditUserProvider.SetCurrentUser(...)`.
2. Paths under `/swagger`, `/scalar`, `/health`, and `/graphql` **bypass** validation; they use the audit user `"system"`.
3. For all other paths, if **X-User-Id** is missing or empty, the middleware returns **400 Bad Request** and does not call the next middleware.
4. **AuditSaveChangesInterceptor** (EF Core) runs on `SaveChanges`/`SaveChangesAsync`. For entities implementing **IAuditableEntity**, it sets:
   - **Added**: CreatedBy, CreatedDatetime, UpdatedBy, UpdatedDatetime
   - **Modified**: UpdatedBy, UpdatedDatetime
   - **Deleted**: IsDeleted = true (soft delete), UpdatedBy, UpdatedDatetime

The application and command/query layers do **not** set audit fields manually; the interceptor is the single place that applies them, using `IAuditUserProvider.GetCurrentUser()`.

### 8.2 Header Requirement

For REST and GraphQL requests that create or update data, send:

```http
X-User-Id: your-user-id
```

---

## 9. How to Run

### 9.1 Prerequisites

- .NET 9 SDK

### 9.2 Run the API

From the repository root:

```bash
dotnet run --project src/presentation/G360.Orders.Presentation.WebApi/G360.Orders.Presentation.WebApi.csproj
```

Or open `G360.Orders.sln` in Visual Studio / Rider and run **G360.Orders.Presentation.WebApi**.

### 9.3 URLs (replace port with actual)

| Purpose | URL |
|---------|-----|
| REST API | `https://localhost:<port>/api/orders`, `/api/ingredients`, etc. |
| Scalar (REST docs) | `https://localhost:<port>/scalar/v1` |
| GraphQL | `https://localhost:<port>/graphql` |

### 9.4 Database

- Default: SQLite file **`orders.db` in the Infrastructure project folder** (`src/G360.Orders.Infrastructure/orders.db`).
- Connection string can be overridden in `appsettings.json` / `appsettings.Development.json` (`ConnectionStrings:DefaultConnection`).
- On startup, migrations may be applied depending on the current `Program.cs` configuration.

### 9.5 Migrations

Add a migration (from repo root):

```bash
dotnet ef migrations add YourMigrationName \
  --project src/G360.Orders.Infrastructure/G360.Orders.Infrastructure.csproj \
  --startup-project src/presentation/G360.Orders.Presentation.WebApi/G360.Orders.Presentation.WebApi.csproj \
  --context OrdersDbContext \
  --output-dir Data/Migrations
```

Apply migrations: run the app (it calls `MigrateAsync()`) or:

```bash
dotnet ef database update --project src/G360.Orders.Infrastructure/... --startup-project src/presentation/...
```

---

## 10. Testing

```bash
dotnet test G360.Orders.sln
```

Test projects: **G360.Orders.Domain.Test**, **G360.Orders.Application.Test**, **G360.Orders.Infrastructure.Test** (xUnit).

---

## 11. Design Decisions (Interview Talking Points)

1. **Clean Architecture** — Domain has no external dependencies; use cases live in Application; persistence and HTTP are in Infrastructure and Presentation. This keeps the core testable and swappable (e.g. different DB or UI).

2. **CQRS with MediatR** — Clear separation between reads and writes; handlers are single-purpose and easy to unit test; new features often mean a new command/query + handler without touching controllers/endpoints.

3. **Audit in one place** — `AuditSaveChangesInterceptor` plus `X-User-Id` from middleware ensures every save gets consistent audit data without duplicating logic in handlers or endpoints.

4. **Dual API (REST + GraphQL)** — REST (FastEndpoints) for simple, resource-oriented clients; GraphQL for clients that need flexible shapes and a single endpoint. Both use the same Application layer.

5. **Repository + UoW** — DbContext is the unit of work; repositories encapsulate queries and add/update/delete. Specialized interfaces (`IOrderRepository`, `IPizzaDetailRepository`) support aggregate operations (e.g. replacing order details or pizza-type ingredients).

6. **Soft delete** — Handled in the interceptor (IsDeleted + UpdatedBy/UpdatedDatetime). List endpoints can filter by `includeDeleted`; business logic can ignore or include soft-deleted data as needed.

7. **Validation** — FluentValidation in the Application layer validates commands/queries before handlers run, keeping validation rules centralized and testable.

---

## 12. Quick Reference

- **REST base**: `/api` (kebab-case: `/api/pizza-types`, `/api/orders`, etc.)
- **GraphQL**: `/graphql` (Banana Cake Pop for docs and playground)
- **REST docs**: `/scalar/v1`
- **Header**: `X-User-Id` required for non-bypass paths
- **DB**: SQLite, migrations in Infrastructure, applied on startup

For sample GraphQL operations, see **docs/GraphQL-Samples.graphql**.
