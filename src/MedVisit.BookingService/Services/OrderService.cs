using MedVisit.BookingService.Entities;
using MedVisit.BookingService.Enums;
using MedVisit.BookingService.Models;
using MedVisit.BookingService.RabbitMq;
using System.Net.Http;

namespace MedVisit.BookingService.Services
{
    public interface IOrderService
    {
        Task<CreateOrderResult> CreateOrderAsync(int userId, CreateOrderRequest request);
    }

    public class OrderService : IOrderService
    {
        private readonly BookingDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly EventPublisher _eventPublisher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderService(BookingDbContext dbContext, IHttpClientFactory httpClientFactory, EventPublisher eventPublisher, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
            _eventPublisher = eventPublisher;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CreateOrderResult> CreateOrderAsync(int userId, CreateOrderRequest request)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("PaymentService");
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var paymentResponse = await httpClient.PostAsJsonAsync("withdraw", new { UserId = userId, Amount = request.OrderAmount });
                Console.WriteLine(paymentResponse);
                if (!paymentResponse.IsSuccessStatusCode)
                {
                    await _eventPublisher.PublishAsync("booking_exchange", new
                    {
                        UserId = userId,
                        Status = "Failed",
                        Message = $"Не удалось оплатить {request.ServiceName} со стоимостью {request.OrderAmount:C}. Недостаточно средств или произошла ошибка платежа.",
                        Subject = "Ошибка оплаты",
                        Timestamp = DateTime.UtcNow
                    });

                    return new CreateOrderResult
                    {
                        OrderId = 0,
                        Success = false,
                        Message = "Оплата не прошла. Заказ не может быть создан."
                    };
                }

                var order = new OrderDb
                {
                    UserId = userId,
                    Amount = request.OrderAmount,
                    ServiceName = request.ServiceName,
                    CreatedAt = DateTime.UtcNow,
                    Status = OrderStatus.Created
                };

                await _dbContext.Orders.AddAsync(order);
                await _dbContext.SaveChangesAsync();

                await _eventPublisher.PublishAsync("booking_exchange", new
                {
                    UserId = userId,
                    Status = "Success",
                    Message = $"Оплата услуги успешно завершена. Номер заказа: {order.Id}, Сумма: {request.OrderAmount:C}.",
                    Subject = "Успешная оплата",
                    Timestamp = DateTime.UtcNow
                });


                return new CreateOrderResult
                {
                    OrderId = order.Id,
                    Success = true,
                    Message = "Заказ успешно создан."
                };
            }
            catch (Exception ex)
            {
                return new CreateOrderResult
                {
                    OrderId = 0,
                    Success = false,
                    Message = $"Произошла ошибка: {ex.Message}"
                };
            }
        }

    }
}
