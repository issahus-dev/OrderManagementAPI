# Order Management API

A REST API built with **.NET 10** and **Entity Framework Core 10** that allows users to create and retrieve customer orders, with configurable business rules enforced at the service layer.

---

## 1. Build and Run Instructions

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

No database setup is required. The app uses **SQLite** and automatically creates and migrates the database (`app.db`) on first startup.

### Run the API

```bash
cd OrderManagementAPI
dotnet run
```

The API starts on `http://localhost:5001`.
Swagger UI is available at: `http://localhost:5001/swagger` (Development environment only).

The SQLite database path will be printed to the console on startup:

```
SQLite DB PATH: app.db
```

### Adjust the Order Value Limit

Open `appsettings.json` and update `MaxOrderValueLimit`:

```json
"OrderSettings": {
  "MaxOrderValueLimit": 50000.00
}
```

No rebuild is required — the value is read from configuration at startup. The default fallback value (if not set in config) is `10`.

### Try the API

A `.http` file is included for use in Visual Studio or VS Code with the REST Client extension:

```
OrderManagementAPI.http
```

Or use curl:

```bash
# Create an order
curl -X POST http://localhost:5001/orders \
  -H "Content-Type: application/json" \
  -d '{"customerName":"Acme Ltd","orderValue":250.00,"orderDate":"2026-01-15"}'

# Get all orders
curl http://localhost:5001/orders

# Get a specific order
curl http://localhost:5001/orders/1
```

---

## 2. Assumptions

- **Order date is date-only.** `DateOnly` is used rather than `DateTime` — time-of-day is not relevant to any business rule.
- **Order value must be positive.** A zero or negative order value is rejected as invalid input.
- **Customer name matching is case-sensitive.** `"Acme Ltd"` and `"acme ltd"` are treated as different customers.
- **No authentication.** No auth has been added as it was not part of the requirements.
- **`CreatedAt` is server-assigned.** The server stamps the creation timestamp in UTC — clients do not supply it.
- **The duplicate check spans all three fields.** An exact match on customer name, order date, *and* order value is required to trigger a conflict. Two orders from the same customer on the same day with different values are both valid.

---

## 3. Design Decisions

### Project Structure

```
OrderManagementAPI/
├── Controllers/        # HTTP layer — routing and mapping service outcomes to status codes
├── Services/           # Business logic (IOrderService / OrderService)
├── Interface/          # Service interface definitions
├── Data/               # AppDbContext and EF Core configuration
├── Security/           # CustomExceptionFilterAttribute
├── OrderSettings.cs    # Strongly-typed configuration binding
├── appsettings.json              # Production configuration
├── appsettings.Development.json  # Development overrides
└── Program.cs          # App bootstrap, DI registration, middleware pipeline
```

Controllers are intentionally thin — they delegate all decisions to `IOrderService` and are only responsible for translating service outcomes into HTTP responses. This keeps business logic decoupled from the HTTP layer and straightforward to test in isolation.

### Data Access

**Entity Framework Core 10 with SQLite** was chosen because:

- **Zero setup.** SQLite requires no server installation or configuration. The database is a single file (`app.db`) that EF Core creates automatically.
- **Automatic migrations.** `database.Database.Migrate()` runs on startup, keeping the schema up to date without any manual steps.

The connection string is driven entirely by configuration (`ConnectionStrings:ConnectionString` in `appsettings.json`), making it straightforward to point at a different data source per environment without any code changes.

### Validation Approach

Validation is split deliberately across two layers:

| Layer | Responsibility |
|---|---|
| **Data Annotations** on request DTOs | Structural validity — required fields, length limits, value ranges. Handled automatically by the model binding pipeline and returned as `400 Bad Request`. |
| **`OrderService`** | Business rule validation — `MaxOrderValueLimit` enforcement and duplicate order detection. Returned as typed results rather than exceptions, keeping control flow predictable and easy to test. |

Separating structural validation (is this a well-formed request?) from business validation (does this request make sense for our domain?) means each layer has a single, clear responsibility. Expected business outcomes like duplicate orders or value limit breaches are not exceptional events and should not be modelled as exceptions.

### Error Handling

- **`CustomExceptionFilterAttribute`** is registered globally via `options.Filters.Add<CustomExceptionFilterAttribute>()`. It catches any unhandled exception and returns a generic `500` response, ensuring internal server details are never exposed to callers.
- **`404 Not Found`** is returned by the controller when a requested order does not exist.
- **`409 Conflict`** is used for duplicate orders — the request is well-formed, but conflicts with existing state, making 409 semantically more accurate than 400.
- **`422 Unprocessable Entity`** is used when a structurally valid request violates a business rule (e.g. order value exceeds `MaxOrderValueLimit`), clearly distinguishing business rule failures from malformed input errors.

---

## 4. Dependencies

| Package | Version | Purpose |
|---|---|---|
| `Microsoft.EntityFrameworkCore` | 10.0.9 | ORM and DbContext |
| `Microsoft.EntityFrameworkCore.Sqlite` | 10.0.9 | SQLite database provider |
| `Microsoft.EntityFrameworkCore.Design` | 10.0.9 | EF Core CLI tooling (migrations) |
| `Microsoft.AspNetCore.OpenApi` | 10.0.2 | OpenAPI document generation |
| `Swashbuckle.AspNetCore` | 10.2.3 | Swagger UI |

---

## 5. Future Improvements

1. **Input normalisation** — Trimming whitespace from `CustomerName` before persisting and before duplicate-checking would make the duplicate rule more robust against accidental differences.

2. **Authentication** — JWT bearer or API key authentication would be required before any production deployment.
