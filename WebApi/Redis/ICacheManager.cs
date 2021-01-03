namespace WebApi.Redis
{
    public interface ICacheManager
    {
        T Get<T>(string key);
        object Get(string key);
        void Add(string key, object data, int duration);
        bool Any(string key);
        void Remove(string key);
    }
}
