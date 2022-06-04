using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApi.RabbitMQ;

namespace WebApi
{
    public class UserConsumerService : BackgroundService
    {
        private readonly ILogger<UserConsumerService> _logger;
        private readonly RabbitMqService _rabbitMqService;

        public UserConsumerService(ILogger<UserConsumerService> logger, RabbitMqService rabbitMqService)
        {
            _logger = logger;
            _rabbitMqService = rabbitMqService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CreateQueue();
            ConsumeQueue();
            await Task.CompletedTask;
        }
        private void CreateQueue()
        {
            using var model = _rabbitMqService.CreateModel();
            model.QueueDeclare("user", false);
        }
        private void ConsumeQueue()
        {
            _logger.LogInformation("Listening message queue...");
            string message = "";

            var channel = _rabbitMqService.CreateModel();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                message = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation("User:{@user}", message);
            };
            channel.BasicConsume(queue: "user", autoAck: false, consumer: consumer);
        }
    }
}
