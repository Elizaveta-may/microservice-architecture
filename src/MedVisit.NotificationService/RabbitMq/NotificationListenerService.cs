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
            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync("booking_exchange", ExchangeType.Fanout);

            var queueName = "notification_queue";
            await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            await channel.QueueBindAsync(queueName, "booking_exchange", "");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
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

            channel.BasicConsumeAsync(queueName, autoAck: true, consumer);

            // Keep the service running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }

}
