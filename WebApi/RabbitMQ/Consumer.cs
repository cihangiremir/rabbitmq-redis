using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace WebApi.RabbitMQ
{
    public class Consumer
    {
        private readonly RabbitMQService _rabbitMQService;
        public string Message { get; private set; }
        public Consumer(string queueName)
        {
            _rabbitMQService = new RabbitMQService();

            using (var connection = _rabbitMQService.GetRabbitMQConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var consumer = new EventingBasicConsumer(channel);
                    // Received event'i sürekli listen modunda olacaktır.
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        Message = Encoding.UTF8.GetString(body);
                    };

                    channel.BasicConsume(queueName, true, consumer);
                }
            }
        }
    }
}
