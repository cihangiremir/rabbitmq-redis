using Newtonsoft.Json;
using System;

namespace WebApi.Redis
{
    public class RedisCacheManager : ICacheManager
    {
        private IRedisServer _redisServer;
        public RedisCacheManager(IRedisServer redisServer)
        {
            _redisServer = redisServer;
        }
        public void Add(string key, object data, int duration)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            _redisServer.Database.StringSet(key, jsonData, TimeSpan.FromMinutes(duration));
        }

        public T Get<T>(string key)
        {
            if (Any(key))
            {
                string jsonData = _redisServer.Database.StringGet(key);
                return JsonConvert.DeserializeObject<T>(jsonData);
            }
            return default;
        }

        public object Get(string key)
        {
            if (Any(key))
            {
                string jsonData = _redisServer.Database.StringGet(key);
                return JsonConvert.DeserializeObject<object>(jsonData);
            }
            return default;
        }

        public bool Any(string key)
        {
            return _redisServer.Database.KeyExists(key);
        }

        public void Remove(string key)
        {
            _redisServer.Database.KeyDelete(key);
        }
    }
}
