using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderManagementAPI;
using OrderManagementAPI.Data;
using OrderManagementAPI.Interface;
using OrderManagementAPI.Security;
using OrderManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);
//Max limit
builder.Services.Configure<OrderSettings>(builder.Configuration.GetSection("OrderSettings"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ConnectionString")));



builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddControllers();
// Custom exceptions to not expose any server details within exception message. 

builder.Services.AddControllers(options => options.Filters.Add<CustomExceptionFilterAttribute>());

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

//Ensure the DB is migrated
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    database.Database.Migrate();
    var dbPath = database.Database.GetDbConnection().DataSource;
    Console.WriteLine($"SQLite DB PATH: {dbPath}");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Order Management API");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
