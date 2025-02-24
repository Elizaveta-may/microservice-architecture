using MedVisit.BookingService.Models;
using MedVisit.BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Prometheus;
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

        [HttpPost("generate-idempotency-key")]
        [Authorize(Roles = "User")]
        public IActionResult GenerateIdempotencyKey()
        {
            var idempotencyKey = Guid.NewGuid().ToString();
            return Ok(new { IdempotencyKey = idempotencyKey });
        }

        [HttpPost("booking")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Booking([FromBody] OrderRequest request)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            TotalBookingRequests.Labels("booking").Inc(); 

            if (!ModelState.IsValid)
            {
                FailedBookings.Labels("booking").Inc(); 
                return BadRequest(ModelState);
            }

            var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (string.IsNullOrEmpty(idempotencyKey))
            {
                FailedBookings.Labels("booking").Inc(); 
                return BadRequest(new { message = "Idempotency-Key is required." });
            }

            var cachedResult = await _redisDb.StringGetAsync(idempotencyKey);
            if (cachedResult.HasValue)
            {
                stopwatch.Stop();
                BookingLatency.Labels("booking").Observe(stopwatch.Elapsed.TotalSeconds); 
                return Ok(new { message = "Заказ уже создан ранее.", result = cachedResult.ToString() });
            }

            var userId = User.FindFirst("user_id")?.Value;

            var orderResult = await _orderService.ProcessOrderAsync(int.Parse(userId), request);

            if (!orderResult.IsSuccess)
            {
                FailedBookings.Labels("booking").Inc(); 
                stopwatch.Stop();
                BookingLatency.Labels("booking").Observe(stopwatch.Elapsed.TotalSeconds); 
                return BadRequest(new { message = orderResult.Message });
            }

            await _redisDb.StringSetAsync(idempotencyKey, JsonConvert.SerializeObject(orderResult), TimeSpan.FromHours(1));

            stopwatch.Stop();
            BookingLatency.Labels("booking").Observe(stopwatch.Elapsed.TotalSeconds); 

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
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            TotalBookingRequests.Labels("cancelBooking").Inc(); 

            var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (string.IsNullOrEmpty(idempotencyKey))
            {
                FailedBookings.Labels("cancelBooking").Inc();
                stopwatch.Stop();
                BookingLatency.Labels("cancelBooking").Observe(stopwatch.Elapsed.TotalSeconds); 
                return BadRequest(new { message = "Idempotency-Key is required." });
            }

            var cachedResult = await _redisDb.StringGetAsync(idempotencyKey);
            if (cachedResult.HasValue)
            {
                stopwatch.Stop();
                BookingLatency.Labels("cancelBooking").Observe(stopwatch.Elapsed.TotalSeconds);
                return Ok(new { message = "Заказ уже был отменен ранее.", result = cachedResult.ToString() });
            }

            var userId = User.FindFirst("user_id")?.Value;

            var orderResult = await _orderService.ProcessCancelOrderAsync(int.Parse(userId), orderId);

            if (orderResult.OrderId == 0)
            {
                FailedBookings.Labels("cancelBooking").Inc(); 
                stopwatch.Stop();
                BookingLatency.Labels("cancelBooking").Observe(stopwatch.Elapsed.TotalSeconds);
                return BadRequest(new { message = "Ошибка отмены бронирования." });
            }

            await _redisDb.StringSetAsync(idempotencyKey, JsonConvert.SerializeObject(orderResult), TimeSpan.FromHours(1));

            stopwatch.Stop();
            BookingLatency.Labels("cancelBooking").Observe(stopwatch.Elapsed.TotalSeconds);

            return Ok(new
            {
                message = "Бронь успешно отменена.",
                orderId = orderResult.OrderId
            });
        }

        private static readonly Histogram BookingLatency = Metrics.CreateHistogram(
            "booking_request_duration_seconds",
            "Время выполнения запроса на создание бронирования.",
            new HistogramConfiguration
            {
                LabelNames = new[] { "method" }
            });

        private static readonly Counter TotalBookingRequests = Metrics.CreateCounter(
            "booking_requests_total",
            "Общее количество запросов на создание бронирования.",
            new CounterConfiguration
            {
                LabelNames = new[] { "method" }
            });

        private static readonly Counter FailedBookings = Metrics.CreateCounter(
            "failed_bookings_total",
            "Общее количество неуспешных бронирований.",
            new CounterConfiguration
            {
                LabelNames = new[] { "method" }
            });
    }
}
