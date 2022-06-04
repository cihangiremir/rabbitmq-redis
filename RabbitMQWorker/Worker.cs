using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQConsumerWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private IConnection _connection;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CreateConnection();
            CreateQueue();
            ConsumeQueue();
            await Task.CompletedTask;
        }
        private void CreateConnection()
        {
            _logger.LogInformation("Creating rabbitmq connection");
            var factory = new ConnectionFactory()
            {
                HostName = _configuration.GetValue<string>("RabbitMQHost"),
            };
            _connection = factory.CreateConnection();
            _logger.LogInformation("Created rabbitmq connection");
        }
        private void CreateQueue()
        {
            var model = _connection.CreateModel();
            model.QueueDeclare("message", false);
        }
        private void ConsumeQueue()
        {
            _logger.LogInformation("Listening message queue...");
            string message = "";

            var channel = _connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                message = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation("Message:{@message}", message);
            };
            channel.BasicConsume(queue: "message", autoAck: false, consumer: consumer);
        }
    }
}
