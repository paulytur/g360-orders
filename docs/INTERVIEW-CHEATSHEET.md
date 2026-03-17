# G360 Orders — Interview Cheat Sheet

One-page summary for presenting the project.

---

## What It Is

A **pizza ordering management API** (.NET 9) with **REST** and **GraphQL**, **audit fields**, and **soft delete**. Built with **Clean Architecture** and **CQRS**.

---

## Stack (30 sec)

| Layer / concern | Tech |
|-----------------|------|
| API | FastEndpoints (REST) + HotChocolate (GraphQL) |
| Docs | Scalar (REST), Banana Cake Pop (GraphQL) |
| Core | MediatR (CQRS), FluentValidation, AutoMapper |
| Data | EF Core 9, SQLite, migrations |
| Tests | xUnit |

---

## Architecture (1 min)

- **Domain** — Entities (Order, OrderDetail, Pizza, PizzaType, PizzaDetail, Ingredient, Category) + interfaces. No dependencies.
- **Application** — Commands/Queries, Handlers, Validators. Depends on Domain only.
- **Infrastructure** — DbContext, repositories, audit interceptor. Implements Domain interfaces.
- **Presentation** — Endpoints, GraphQL types, middleware. Wires everything; depends on Application + Infrastructure.

**CQRS:** Every use case is a Command or Query; MediatR dispatches to a single handler. Handlers use repositories and return `Response<T>` or `PagedResponse<T>`.

---

## Audit (30 sec)

- **X-User-Id** header = “who is doing this.”
- **Middleware** reads the header and sets the current user (or returns 400 if missing). Paths like `/graphql`, `/scalar` bypass and use `"system"`.
- **EF interceptor** on SaveChanges sets CreatedBy/UpdatedBy/CreatedDatetime/UpdatedDatetime and soft delete (IsDeleted). One place, no logic in handlers.

---

## API at a Glance

- **REST:** `/api/orders`, `/api/ingredients`, `/api/categories`, `/api/pizzas`, `/api/pizza-types` — GET (list + by id), POST, PUT, DELETE. Kebab-case.
- **GraphQL:** `/graphql` — same operations as queries/mutations (e.g. `getOrders`, `createCategory`). Samples in `docs/GraphQL-Samples.graphql`.

---

## Design Decisions to Mention

1. **Clean Architecture** — Testable core; persistence and HTTP are replaceable.
2. **CQRS** — Clear read/write split; easy to add features and test handlers.
3. **Audit in interceptor** — No repeated code; every save is consistent.
4. **REST + GraphQL** — Same Application layer; different clients can choose.
5. **Repository pattern** — Domain defines interfaces; Infrastructure implements; Application stays persistence-agnostic.
6. **Soft delete** — Interceptor sets IsDeleted; list endpoints can include/exclude via parameter.

---

## Run & Test

```bash
dotnet run --project src/presentation/G360.Orders.Presentation.WebApi/G360.Orders.Presentation.WebApi.csproj
dotnet test G360.Orders.sln
```

**Full doc:** [docs/DOCUMENTATION.md](DOCUMENTATION.md)
