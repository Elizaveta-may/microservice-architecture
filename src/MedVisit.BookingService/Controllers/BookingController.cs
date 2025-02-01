using MedVisit.BookingService.Models;
using MedVisit.BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using System.Security.Claims;

namespace MedVisit.BookingService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly StackExchange.Redis.IDatabase _redisDb;

        public BookingController(IOrderService orderService, IConnectionMultiplexer redis)
        {
            _orderService = orderService;
            _redisDb = redis.GetDatabase();
        }

        [HttpPost("booking")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Booking([FromBody] OrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (string.IsNullOrEmpty(idempotencyKey))
            {
                return BadRequest(new { message = "Idempotency-Key is required." });
            }

            var cachedResult = await _redisDb.StringGetAsync(idempotencyKey);
            if (cachedResult.HasValue)
            {
                return Ok(new { message = "Заказ уже создан ранее.", result = cachedResult.ToString() });
            }
            var userId = User.FindFirst("user_id")?.Value;

            var orderResult = await _orderService.ProcessOrderAsync(int.Parse(userId), request);

            if (!orderResult.IsSuccess)
            {
                return BadRequest(new { message = orderResult.Message });
            }

            await _redisDb.StringSetAsync(idempotencyKey, "Заказ успешно создан.", TimeSpan.FromHours(1));

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
