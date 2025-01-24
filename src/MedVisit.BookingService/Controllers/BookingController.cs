using MedVisit.BookingService.Models;
using MedVisit.BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedVisit.BookingService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public BookingController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst("user_id")?.Value;

            var orderResult = await _orderService.CreateOrderAsync(int.Parse(userId), request);

            if (orderResult.OrderId == 0)
            {
                return BadRequest(new { message = "Недостаточно средств." });
            }

            return Ok(new
            {
                message = "Заказ успешно создан.",
                orderId = orderResult.OrderId
            });
        }
    }
}
