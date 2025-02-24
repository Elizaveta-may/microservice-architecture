using System.Text;
using System.Text.Json;
using MedVisit.NotificationService.Entities;
using MedVisit.NotificationService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MedVisit.NotificationService.RabbitMq
{
    public class NotificationListenerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public NotificationListenerService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMq:Host"],
                UserName = Environment.GetEnvironmentVariable("RABBIT_USER"),
                Password = Environment.GetEnvironmentVariable("RABBIT_PASSWORD")
            };

            var connection = await factory.CreateConnectionAsync();

            var bookingChannel = await connection.CreateChannelAsync();
            var authChannel = await connection.CreateChannelAsync();

            await bookingChannel.ExchangeDeclareAsync("booking_exchange", ExchangeType.Fanout);
            await authChannel.ExchangeDeclareAsync("auth_exchange", ExchangeType.Fanout);

            var bookingQueueName = "notification_queue";
            await bookingChannel.QueueDeclareAsync(bookingQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var authQueueName = "auth_notification_queue";
            await authChannel.QueueDeclareAsync(authQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            await bookingChannel.QueueBindAsync(bookingQueueName, "booking_exchange", "");
            await authChannel.QueueBindAsync(authQueueName, "auth_exchange", "");

            var bookingConsumer = new AsyncEventingBasicConsumer(bookingChannel);
            bookingConsumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var notificationMessage = JsonSerializer.Deserialize<Notification>(message);

                if (notificationMessage != null)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

                    var entity = new NotificationDb
                    {
                        UserId = notificationMessage.UserId,
                        Subject = notificationMessage.Subject,
                        Message = notificationMessage.Message
                    };

                    await dbContext.Notifications.AddAsync(entity);
                    await dbContext.SaveChangesAsync();
                }
            };

            var authConsumer = new AsyncEventingBasicConsumer(authChannel);
            authConsumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var notificationMessage = JsonSerializer.Deserialize<Notification>(message);

                if (notificationMessage != null)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

                    var entity = new NotificationDb
                    {
                        UserId = notificationMessage.UserId,
                        Subject = notificationMessage.Subject,
                        Message = notificationMessage.Message
                    };

                    await dbContext.Notifications.AddAsync(entity);
                    await dbContext.SaveChangesAsync();
                }
            };

            await bookingChannel.BasicConsumeAsync(bookingQueueName, autoAck: true, bookingConsumer);
            await authChannel.BasicConsumeAsync(authQueueName, autoAck: true, authConsumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

    }

}
