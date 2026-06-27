using OrderManagementAPI.Models;

namespace OrderManagementAPI.Interface
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerByCustomerIDAsync(int customerID);
        Task<IEnumerable<Customer>> ListAllCustomersAsync();
        Task<SaveResponse<Customer>> CreateCustomerAsync(CreateCustomer customer);

    }
}
