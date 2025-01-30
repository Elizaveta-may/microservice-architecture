using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace MedVisit.BookingService.RabbitMq
{
    public class EventPublisher
    {
        private readonly IConfiguration _configuration;

        public EventPublisher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task PublishAsync(string exchangeName, object message)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMq:Host"],
                UserName = Environment.GetEnvironmentVariable("RABBIT_USER"),
                Password = Environment.GetEnvironmentVariable("RABBIT_PASSWORD")
            };
            try
            {
                await using var connection = await factory.CreateConnectionAsync();
                await using var channel = await connection.CreateChannelAsync();

                await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Fanout);

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                var properties = new BasicProperties();
                properties.ContentType = "application/json";

                await channel.BasicPublishAsync(
                    exchange: exchangeName,
                    routingKey: "",
                    mandatory: false,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: CancellationToken.None);

            }
            catch (BrokerUnreachableException ex)
            {
                Console.WriteLine($"RabbitMQ unreachable: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while publishing the message: {ex.Message}");
            }
        }
    }

}