using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace WebApi.Redis
{
    public class RedisServer : IRedisServer
    {
        private ConnectionMultiplexer _connectionMultiplexer;
        private int _currentDatabaseId = 0;
        private IDatabase _database;
        public IDatabase Database => _database;
        private readonly IOptions<RedisConfiguration> _redisConfiguration;
        public RedisServer(IConfiguration configuration, IOptions<RedisConfiguration> redisConfiguration)
        {
            Connect();
            _redisConfiguration = redisConfiguration;
        }
        private void Connect()
        {
            _connectionMultiplexer = ConnectionMultiplexer.Connect($"{_redisConfiguration.Value.Host}:{_redisConfiguration.Value.Port}");
            _database = _connectionMultiplexer.GetDatabase(_currentDatabaseId);
        }
        public void FlushDatabase()
        {
            _connectionMultiplexer.GetServer($"{_redisConfiguration.Value.Host}:{_redisConfiguration.Value.Port}").FlushDatabase(_currentDatabaseId);
        }
        public void FlushAllDatabases()
        {
            _connectionMultiplexer.GetServer($"{_redisConfiguration.Value.Host}:{_redisConfiguration.Value.Port}").FlushAllDatabases();
        }
    }
}
