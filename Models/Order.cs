namespace OrderManagementAPI.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal OrderValue { get; set; }
        public DateOnly OrderDate {  get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    }
}
