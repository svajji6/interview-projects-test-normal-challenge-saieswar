using Microsoft.AspNetCore.Mvc;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Requests;
using SampleAPI.Services;

namespace SampleAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }
        /// <summary>
        /// Get React order details from order table and i added seed data in this application 
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetOrders")] // TODO: Change route, if needed.
        [ProducesResponseType(StatusCodes.Status200OK)] // TODO: Add all response types
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            try
            {
                var recentOrders = await _orderService.GetRecentOrders();
                return Ok(recentOrders);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error retrieving recent orders");
                return StatusCode(500, "Internal server error");
            }
        }
        
        /// TODO: Add an endpoint to allow users to create an order using <see cref="CreateOrderRequest"/>.
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest order)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var createdOrder = await _orderService.SubmitOrder(order);
                return Ok(createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting order");
                return StatusCode(500, "Internal server error");
            }
        }
        
        /// <summary>
        ///  GetOrdersExcludingHolidaysAndWeekends and I wrote only Xunit cases for Controller and didn't write service and repository
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        [HttpGet("days/{days:int}")]
        public async Task<IActionResult> GetOrdersExcludingHolidaysAndWeekends(int days)
        {
            if (days <= 0)
            {
                return BadRequest("Days must be greater than 0.");
            }

            try
            {
                var orders = await _orderService.GetOrdersExcludingHolidaysAndWeekends(days);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

