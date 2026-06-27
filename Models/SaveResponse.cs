namespace OrderManagementAPI.Models
{
    public class SaveResponse<T>
    {
        public bool Success { get; set; }
        
        public T Payload { get; set; }
        public SaveError Error { get; set; }
        public string Message { get; set; } = string.Empty;
    }
    public enum SaveError
    {
        Duplicate,
        OrderExceedsMaxLimit
    }
}
