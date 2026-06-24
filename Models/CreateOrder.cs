using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.Models
{
    public record CreateOrderRequest
    {
        [Required(ErrorMessage = "Customer name is required.")]
        [MaxLength(200, ErrorMessage = "Customer name must not exceed 200 characters.")]
        public string CustomerName { get; init; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Order value must be greater than zero.")]
        public decimal OrderValue { get; init; }

        public DateOnly OrderDate { get; init; }
    }
}
