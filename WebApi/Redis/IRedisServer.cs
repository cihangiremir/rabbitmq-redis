using StackExchange.Redis;

namespace WebApi.Redis
{
    public interface IRedisServer
    {
        public IDatabase Database { get; }
        public void FlushDatabase();
        public void FlushAllDatabases();
    }
}
