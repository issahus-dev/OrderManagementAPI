namespace OrderManagementAPI.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int Age { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
