using MedVisit.BookingService.Entities;
using MedVisit.BookingService.Enums;
using MedVisit.BookingService.Models;
using MedVisit.BookingService.RabbitMq;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace MedVisit.BookingService.Services
{
    public interface ISagaStepsService {
        Task<bool> PayForService(int userId, decimal amount, string serviceName);
        Task<bool> RefundPayment(int userId, decimal amount);
        Task<bool> BookTimeSlot(int userId, int timeSlotId);
        Task<bool> CancelTimeSlot(int userId, int timeSlotId);
        Task<bool> SaveBookingToDatabase(int userId, OrderRequest request);
        Task<bool> UpdateOrderStatusInDatabase(int orderId, OrderStatus status);
        Task RemoveBookingFromDatabase(int userId, OrderRequest request);
        Task<bool> SendSuccessBookingNotification(int userId, OrderRequest request);
        Task<bool> SendSuccessCancelBookingNotification(int userId, OrderRequest request);
    }


    public class SagaStepsService : ISagaStepsService
    {
        private readonly BookingDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly EventPublisher _eventPublisher;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SagaStepsService(BookingDbContext dbContext, IHttpClientFactory httpClientFactory, EventPublisher eventPublisher, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext; 
            _httpClientFactory = httpClientFactory;
            _eventPublisher = eventPublisher;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> PayForService(int userId, decimal amount, string serviceName)
        {
            try
            {
                var httpClient = CreateHttpClient("PaymentService");

                var response = await httpClient.PostAsJsonAsync("withdraw", new { UserId = userId, Amount = amount });

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await PublishEvent(userId, "Failed", $"Ошибка оплаты услуги {serviceName}: {errorMessage}", "Ошибка оплаты");
                    return false;
                }

                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        public async Task<bool> RefundPayment(int userId, decimal amount)
        {
            try
            {
                var httpClient = CreateHttpClient("PaymentService");
                var response = await httpClient.PostAsJsonAsync("deposit", new { UserId = userId, Amount = amount });

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await PublishEvent(userId, "Failed", $"Ошибка  возврата средств: {errorMessage}", "Ошибка оплаты");
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> BookTimeSlot(int userId,int timeSlotId)
        {
            try
            {
                var httpClient = CreateHttpClient("ScheduleService");

                var response = await httpClient.PutAsJsonAsync($"BookSlot?availabilityId={timeSlotId}", new { });

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await PublishEvent(userId, "Failed", $"Ошибка бронирования услуги: {errorMessage}", "Ошибка бронирования");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<bool> CancelTimeSlot(int userId, int timeSlotId)
        {
            try
            {
                var httpClient = CreateHttpClient("ScheduleService");

                var response = await httpClient.PutAsJsonAsync($"FreeSlot?availabilityId={timeSlotId}", new { });

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await PublishEvent(userId, "Failed", $"Ошибка бронирования услуги: {errorMessage}", "Ошибка бронирования");
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SaveBookingToDatabase(int userId, OrderRequest request)
        {
            try
            {
                var booking = new OrderDb
                {
                    UserId = userId,
                    MedServiceId = request.MedServiceId,
                    MedServiceName = request.MedServiceName,
                    MedicalWorkerId = request.MedicalWorkerId,
                    MedicalWorkerFullName = request.MedicalWorkerFullName,
                    TimeSlotId = request.TimeSlotId,
                    TimeSlot = request.TimeSlot,
                    Amount = request.Amount,
                    CreatedAt = DateTime.UtcNow,
                    Status = OrderStatus.Completed
                };

                await _dbContext.Orders.AddAsync(booking);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateOrderStatusInDatabase(int orderId, OrderStatus status)
        {
            try
            {
                var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
                order.Status = status;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task RemoveBookingFromDatabase(int userId, OrderRequest request)
        {
            var booking = await _dbContext.Orders
                .Where(b => b.UserId == userId && b.TimeSlotId == request.TimeSlotId)
                .FirstOrDefaultAsync();

            if (booking != null)
            {
                _dbContext.Orders.Remove(booking);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> SendSuccessBookingNotification(int userId, OrderRequest request)
        {
            await PublishEvent(userId, "Success",
                $"Вы успешно записались на услугу {request.MedServiceName} к {request.MedicalWorkerFullName}. Ждем вас {request.TimeSlot}. Сумма: {request.Amount:C}.",
                "Успешная запись на услугу");
            return true;
        }

        public async Task<bool> SendSuccessCancelBookingNotification(int userId, OrderRequest request)
        {
            await PublishEvent(userId, "Success",
                $"Вы успешно отменили запись на {request.MedServiceName} к {request.MedicalWorkerFullName} в {request.TimeSlot}. Сумма {request.Amount:C} возвращена вам на счет.",
                "Успешная отмена записи");
            return true;
        }

        private HttpClient CreateHttpClient(string serviceName)
        {
            var httpClient = _httpClientFactory.CreateClient(serviceName);
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return httpClient;
        }

        private async Task PublishEvent(int userId, string status, string message, string subject)
        {
            await _eventPublisher.PublishAsync("booking_exchange", new
            {
                UserId = userId,
                Status = status,
                Message = message,
                Subject = subject,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
