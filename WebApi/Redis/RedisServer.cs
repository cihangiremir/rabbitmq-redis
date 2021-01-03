using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace WebApi.Redis
{
    public class RedisServer : IRedisServer
    {
        private ConnectionMultiplexer _connectionMultiplexer;
        private RedisConfig redisConfig;
        private int _currentDatabaseId = 0;
        private IDatabase _database;
        public IDatabase Database => _database;
        public RedisServer(IConfiguration configuration)
        {
            redisConfig = configuration.GetSection("RedisConfiguration").Get<RedisConfig>();
            Connect();
        }
        private void Connect()
        {
            _connectionMultiplexer = ConnectionMultiplexer.Connect($"{redisConfig.Host}:{redisConfig.Port}");
            _database = _connectionMultiplexer.GetDatabase(_currentDatabaseId);
        }
        public void FlushDatabase()
        {
            _connectionMultiplexer.GetServer($"{redisConfig.Host}:{redisConfig.Port}").FlushDatabase(_currentDatabaseId);
        }
        public void FlushAllDatabases()
        {
            _connectionMultiplexer.GetServer($"{redisConfig.Host}:{redisConfig.Port}").FlushAllDatabases();
        }
    }
}
