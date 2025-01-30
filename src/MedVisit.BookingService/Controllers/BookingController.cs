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

        [HttpPost("booking")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Booking([FromBody] OrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst("user_id")?.Value;

            var orderResult = await _orderService.ProcessOrderAsync(int.Parse(userId), request);

            if (!orderResult.IsSuccess)
            {
                return BadRequest(new { message = orderResult.Message });
            }

            return Ok(new
            {
                message = "Заказ успешно создан."
            });
        }


        [HttpPut("cancelBooking")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelBooking(int orderId)
        {

            var userId = User.FindFirst("user_id")?.Value;

            var orderResult = await _orderService.ProcessCancelOrderAsync(int.Parse(userId), orderId);

            if (orderResult.OrderId == 0)
            {
                return BadRequest(new { message = "Ошибка отмены бронирования." });
            }

            return Ok(new
            {
                message = "Бронь успешно отменена.",
                orderId = orderResult.OrderId
            });
        }

    }
}
