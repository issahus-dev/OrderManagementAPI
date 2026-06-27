using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderManagementAPI.Data;
using OrderManagementAPI.Interface;
using OrderManagementAPI.Models;
namespace OrderManagementAPI.Services
{
    public class OrderService(AppDbContext database, IOptions<OrderSettings> orderSettings, ILogger<OrderService> logger) : IOrderService
    {
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await database.Orders.OrderByDescending(x => x.CreatedOn).ToListAsync();
        }
        public async Task<Order?> GetOrderByIDAsync(int orderID)
        {
            return await database.Orders.FirstOrDefaultAsync(x => x.OrderID == orderID);
        }
        public async Task<SaveResponse<Order>> CreateOrderAsync(CreateOrderRequest orderRequest)
        {

            SaveError? error = await Validate(orderRequest);

            if (error != null)
            {
                string message = $"Was unable to save order due to {nameof(error.Value)}";
                logger.LogWarning(message);
                return new SaveResponse<Order> { Error = error.Value, Message = message };
            }

            Order order = TransformRequestIntoOrder(orderRequest);

            database.Orders.Add(order);
            await database.SaveChangesAsync();

            logger.LogInformation($"Sucessfully created a new order.ID = {order.OrderID} at = {order.CreatedOn.ToShortTimeString} ");
            
            return new SaveResponse<Order>
            {
                Success = true,
                Payload = order
            };
        }
        private async Task<bool> IsDuplicate(CreateOrderRequest orderRequest)
        {
            return await database.Orders.AnyAsync(x =>
            x.CustomerID == orderRequest.CustomerID &&
            x.OrderValue == orderRequest.OrderValue &&
            x.OrderDate == orderRequest.OrderDate
            );
        }
        private bool IsOrderValueAboveLimit(CreateOrderRequest orderRequest)
        {
            return orderRequest.OrderValue > orderSettings.Value.MaxOrderValueLimit;
        }
        private Order TransformRequestIntoOrder(CreateOrderRequest orderRequest)
        {
             Order order = new Order
            {
                CustomerID = orderRequest.CustomerID,
                OrderValue = orderRequest.OrderValue,
                OrderDate = orderRequest.OrderDate
            };
            return order;
        }
        private async Task<SaveError?> Validate(CreateOrderRequest request)
        {
            if (await IsDuplicate(request))
                return SaveError.Duplicate;

            if (IsOrderValueAboveLimit(request))
                return SaveError.OrderExceedsMaxLimit;

            return null;
        }
    }

}


