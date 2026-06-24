# Order Management API

A REST API built with .NET 10 and Entity Framework Core 10 that allows users to create and retrieve customer orders, with business rules enforced in the service layer.



1. Build and Run Instructions

 Prerequisites

- [.NET 10 SDK]

No database setup is required. The app uses SQLite and automatically creates and migrates the database (`app.db`) on first startup.

To run the API, set the OrderManagementAPI at the startup project and build the solution.


The API starts on `http://localhost:5001`.
Swagger UI is available at: `http://localhost:5001/swagger` (Development environment only).

To adjust the Order Value Limit

Open `appsettings.json` and update `MaxOrderValueLimit`:


To interact with the endpoints use swagger. 

Copy and paste example request body to the /CreateOrders endpoint
 
'{"customerName":"IH test Ltd","orderValue":250.00,"orderDate":"2026-01-15"}'

Get all orders

/Orders

Get a specific order
Orders/1



2. Assumptions

- Order date is date only data type. DateOnly is used rather than time as that was the data type supplied in the example request body
- Order value must be positive. A zero or negative order value is rejected as invalid input.
- No authentication. No auth has been added as it was not part of the requirements.
- The duplicate check spans all three fields. An exact match on customer name, order date, and order value is required to trigger a conflict. 
- Two orders from the same customer on the same day with different values are both valid.



3. Design Decisions

Project Structure


Under the OrderManagementAPI web project I've created folders to categories related classes.

To seperate dependencies I've included an interface, service and data classes, the API controller is kept think

Majority of the processes is handled by the service classes. Which fetches data and applies business logic when

Inserting new orders.

I've gone with this approach because it allows me to create automated tests for each endpoint without having to 
run the API making testing in isolation straight forward

Data Access

Entity Framework Core 10 with SQLite was chosen because:

- Zero setup. SQLite requires no server installation or configuration. The database is a single file (`app.db`) that EF Core creates automatically.
- Automatic migrations. `database.Database.Migrate()` runs on startup, keeping the schema up to date without any manual steps.
- Wanted to take this oppertunity to learn how to use EF Entity

Validation Approach

Validation is including in 

- DataAnnotations on request model Structural validity required fields, length limits, value ranges. Handled automatically by the code.
- OrderService Business rule validation on `MaxOrderValueLimit` enforcement and duplicate order detection. Returned as typed results rather than exceptions, keeping control flow predictable and easy to test.
- Added a unqiue index for each column, to prevent duplicate inserts in the table
First validation step checks if the request is valid. And the 2nd step in the business layer checks if the request aligns with business expections and the 3rd check prevents against race condition.





 Error Handling

- CustomExceptionFilterAttribute is registered globally via `options.Filters.Add<CustomExceptionFilterAttribute>()`. It catches any unhandled exception and returns a generic `500` response, ensuring internal server details are never exposed to callers.
- 404 Not Found is returned by the controller when a requested order does not exist.
- 409 Conflict is used for duplicate orders
- 422 Unprocessable Entity is used when a structurally valid request violates a business rule

The reason to include these responses is to clearly outline the failure of the request to the caller.

---

4. Dependencies

| Package | Version | Purpose |
| `Microsoft.EntityFrameworkCore` | 10.0.9 | ORM and DbContext |
| `Microsoft.EntityFrameworkCore.Sqlite` | 10.0.9 | SQLite database provider |
| `Microsoft.EntityFrameworkCore.Design` | 10.0.9 | EF Core CLI tooling (migrations) |
| `Microsoft.AspNetCore.OpenApi` | 10.0.2 | OpenAPI document generation |
| `Swashbuckle.AspNetCore` | 10.2.3 | Swagger UI |



5. Future Improvements

1. Input normalisation — Trimming whitespace from `CustomerName` before persisting and before duplicate-checking would make the duplicate rule more robust against accidental differences.

2. Include Authentication and Authorisation to the API.

3. Add Infrastructure as Code files and deploy API using cloud services
