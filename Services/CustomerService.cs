using OrderManagementAPI.Data;
using OrderManagementAPI.Interface;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Services
{
    public class CustomerService(AppDbContext database, ILogger<CustomerService> logger) : ICustomerService
    {
        public Task<SaveResponse<Customer>> CreateCustomerAsync(CreateCustomer customer)
        {
            throw new NotImplementedException();
        }

        public Task<Customer> GetCustomerByCustomerIDAsync(int customerID)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Customer>> ListAllCustomersAsync()
        {
            throw new NotImplementedException();
        }
    }
}
