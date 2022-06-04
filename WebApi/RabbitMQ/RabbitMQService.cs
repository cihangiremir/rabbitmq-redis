using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;
using WebApi.RabbitMq;

namespace WebApi.RabbitMQ
{
    public class RabbitMqService
    {
        private readonly IOptions<RabbitMqConfiguration> _options;
        private IConnectionFactory _connectionFactory;
        private IConnection _connection;
        public RabbitMqService(IOptions<RabbitMqConfiguration> options)
        {
            _options = options;
            CreateRabbitMqConnection();

        }
        private void CreateRabbitMqConnection()
        {
            _connectionFactory = new ConnectionFactory()
            {
                UserName = _options.Value.UserName,
                Password = _options.Value.Password,
                Port = _options.Value.Port,
                VirtualHost = _options.Value.VirtualHost
            };
            _connection = _connectionFactory.CreateConnection(_options.Value.HostNames.ToList());
        }
        public IModel CreateModel()
        {
            return _connection.CreateModel();
        }
        private void Publish(string exchange, string routingKey, bool mandatory = false, IBasicProperties basicProperties = null, ReadOnlyMemory<byte> body = default)
        {
            using var channel = CreateModel();
            channel.BasicPublish(exchange, routingKey, mandatory, basicProperties, body);
        }
        public void Publish(string exchange, string routingKey, object body, bool mandatory = false, IBasicProperties basicProperties = null)
        {
            var json = JsonConvert.SerializeObject(body);
            Publish(exchange, routingKey, mandatory, basicProperties, Encoding.UTF8.GetBytes(json));
        }
    }
}
