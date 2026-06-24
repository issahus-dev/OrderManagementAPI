using OrderManagementAPI.Models;

namespace OrderManagementAPI.Interface
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIDAsync(int orderID);
        Task<SaveOrderResponse> CreateOrderAsync(CreateOrderRequest orderRequest); 

    }
}
