using MedVisit.BookingService.Entities;
using MedVisit.BookingService.Enums;
using MedVisit.BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.BookingService.Services
{
    public interface IOrderService
    {
        Task<OrderResult> ProcessOrderAsync(int userId, OrderRequest request);
        Task<OrderResult> ProcessCancelOrderAsync(int userId, int orderId);
    }

    public class OrderService : IOrderService
    {
        private readonly BookingDbContext _dbContext;
        private readonly ISagaStepsService _sagaStepsService;

        public OrderService(
            BookingDbContext dbContext,
            ISagaStepsService sagaStepsService)

        {
            _dbContext = dbContext;
            _sagaStepsService = sagaStepsService;
        }

        public async Task<OrderResult> ProcessOrderAsync(int userId, OrderRequest request)
        {
            var saga = new BookingSaga();
            int orderId = 0;
            try
            {
                //TODO later add check data consistency
                saga.AddStep(
                    name: "PaymentMedService",
                    action: async () => await _sagaStepsService.PayForService(userId, request.Amount, request.MedServiceName),
                    compensate: async () => await _sagaStepsService.RefundPayment(userId, request.Amount)
                );

                saga.AddStep(
                    name: "BookTimeSlot",
                    action: async () => await _sagaStepsService.BookTimeSlot(userId, request.TimeSlotId),
                    compensate: async () => await _sagaStepsService.CancelTimeSlot(userId, request.TimeSlotId)
                );

                saga.AddStep(
                    name: "SaveBookingToDatabase",
                    action: async () =>
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

                        orderId = booking.Id; 

                        return true;
                    },
                    compensate: async () => await _sagaStepsService.RemoveBookingFromDatabase(userId, request)
                );

                saga.AddStep(
                    name: "SendNotification",
                    action: async () => await _sagaStepsService.SendSuccessBookingNotification(userId, request),
                    compensate: null
                );

                await saga.ExecuteAsync();

                return new OrderResult
                {
                    OrderId = orderId,
                    IsSuccess = true,
                    Message = "Бронирование успешно завершено."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке заказа: {ex.Message}");

                return new OrderResult
                {
                    OrderId = orderId,
                    IsSuccess = false,
                    Message = $"Ошибка бронирования: {ex.Message}"
                };
            }
        }

        public async Task<OrderResult> ProcessCancelOrderAsync(int userId, int orderId)
        {
            var order = await _dbContext.Orders
                .Where(o => o.Id == orderId)
                .Select(o => new OrderRequest
                {
                    Amount = o.Amount,
                    MedServiceId = o.MedServiceId,
                    MedServiceName = o.MedServiceName,
                    MedicalWorkerId = o.MedicalWorkerId,
                    MedicalWorkerFullName = o.MedicalWorkerFullName,
                    TimeSlotId = o.TimeSlotId,
                    TimeSlot = o.TimeSlot
                })
                .FirstOrDefaultAsync();

            var saga = new BookingSaga();

            try
            {
                saga.AddStep(
                    name: "PayForService",
                    action: async () => await _sagaStepsService.RefundPayment(userId, order.Amount),
                    compensate: async () => await _sagaStepsService.PayForService(userId, order.Amount, order.MedServiceName)
                );

                saga.AddStep(
                    name: "BookTimeSlot",
                    action: async () => await _sagaStepsService.CancelTimeSlot(userId, order.TimeSlotId),
                    compensate: async () => await _sagaStepsService.BookTimeSlot(userId, order.TimeSlotId)
                );

                saga.AddStep(
                    name: "SaveBookingToDatabase",
                    action: async () => await _sagaStepsService.UpdateOrderStatusInDatabase(userId, OrderStatus.Canceled),
                    compensate: async () => await _sagaStepsService.UpdateOrderStatusInDatabase(userId, OrderStatus.Completed)
                );

                saga.AddStep(
                    name: "SendNotification",
                    action: async () => await _sagaStepsService.SendSuccessCancelBookingNotification(userId, order),
                    compensate: null
                );

                await saga.ExecuteAsync();

                return new OrderResult
                {
                    IsSuccess = true,
                    Message = "Бронирование успешно отменено."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке отмены брони: {ex.Message}");

                return new OrderResult
                {
                    IsSuccess = false,
                    Message = $"Ошибка отмены бронирования: {ex.Message}"
                };
            }
        }

    }
}
