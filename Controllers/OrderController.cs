using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Interface;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Controllers
{

    [ApiController]
    [Route("orders")]
    [Produces("application/json")]
    public class OrderController(IOrderService orderService, ILogger<OrderController> logger) : ControllerBase
    {
        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/All")]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Order> orders = await orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
        /// <summary>
        /// Get order by orderID
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/ByOrderID")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByOrderID(Int32 orderID)
        {
            Order? order = await orderService.GetOrderByIDAsync(orderID);
            if (order is null)
            {
                logger.LogInformation("Order {OrderId} not found.", orderID);
                return NotFound(new { message = $"Order with ID {orderID} was not found." });
            }

            return Ok(order);
        }
        /// <summary>
        /// Add an Order
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateOrder")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var result = await orderService.CreateOrderAsync(request);

            if (!result.Success)
            {
                return result.Error switch
                {
                    SaveError.Duplicate => Conflict(new { message = result.Message }),
                    SaveError.OrderExceedsMaxLimit => UnprocessableEntity(new { message = result.Message }),
                    _ => BadRequest(new { message = result.Message })
                };
            }

            return Ok(result.Payload);
        }
    }

}
