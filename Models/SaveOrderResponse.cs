namespace OrderManagementAPI.Models
{
    public class SaveOrderResponse
    {
        public bool Success { get; set; }
        public Order Order { get; set; } = new Order();
        public SaveError Error { get; set; }
        public string Message { get; set; } = string.Empty;
    }
    public enum SaveError
    {
        Duplicate,
        OrderExceedsMaxLimit
    }
}
