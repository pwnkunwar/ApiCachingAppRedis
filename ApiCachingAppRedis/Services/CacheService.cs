
using StackExchange.Redis;
using System.Text.Json;

namespace ApiCachingAppRedis.Services
{
    public class CacheService : ICacheService
    {
        private IDatabase _cacheDB;
        public CacheService()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _cacheDB = redis.GetDatabase();
        }
        public T GetData<T>(string key)
        {
            var value = _cacheDB.StringGet(key);
            if(!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public object RemoveData(string key)
        {
            var _exist = _cacheDB.KeyExists(key);
            if (_exist)
            {
                return _cacheDB.KeyDelete(key);
            }
            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = _cacheDB.StringSet(key, JsonSerializer.Serialize(value), expiryTime);
            return isSet;
        }
    }
}
