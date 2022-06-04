using System.Collections.Generic;

namespace WebApi.RabbitMq
{
    public class RabbitMqConfiguration
    {
        public IEnumerable<string> HostNames { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
    }
}
