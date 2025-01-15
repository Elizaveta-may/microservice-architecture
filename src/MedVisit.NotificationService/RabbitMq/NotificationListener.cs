using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Connections;
using NotificationService.Models;
using NotificationService.Sevices;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace MedVisit.NotificationService.RabbitMq
{
    public class NotificationListener
    {
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;

        public NotificationListener(IConfiguration configuration, INotificationService notificationService)
        {
            _configuration = configuration;
            _notificationService = notificationService;
        }

        public async Task StartListening(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMq:Host"],
                UserName = Environment.GetEnvironmentVariable("RABBIT_USER"),
                Password = Environment.GetEnvironmentVariable("RABBIT_PASSWORD")
            };
            Console.WriteLine("Creds: "+factory.HostName + " " + factory.UserName + " " + factory.Password);

            try
            {
                var connection = await factory.CreateConnectionAsync();
                var channel = await connection.CreateChannelAsync();

                await channel.ExchangeDeclareAsync("booking_exchange", ExchangeType.Fanout);

                var queueName = "notification_queue";
                await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                await channel.QueueBindAsync(queueName, "booking_exchange", "");

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    if (stoppingToken.IsCancellationRequested)
                        return;

                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var notificationMessage = JsonSerializer.Deserialize<Notification>(message);
                    if (notificationMessage != null)
                    {
                        await _notificationService.SaveNotificationAsync(notificationMessage);
                    }
                };

                await channel.BasicConsumeAsync(queueName, autoAck: true, consumer);

               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
