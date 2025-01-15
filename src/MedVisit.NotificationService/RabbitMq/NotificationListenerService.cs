namespace MedVisit.NotificationService.RabbitMq
{
    public class NotificationListenerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationListenerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var listener = scope.ServiceProvider.GetRequiredService<NotificationListener>();

            try
            {
                await listener.StartListening(stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in NotificationListenerService: {ex.Message}");
            }
        }
    }
}
