namespace OrderManagementAPI.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public decimal OrderValue { get; set; }
        public DateOnly OrderDate {  get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public Customer Customer { get; set; } = null!;


    }
}
