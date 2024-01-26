using gigadr3w.msauthflow.sharedcache.Interfaces;
using gigadr3w.msauthflow.sharedcache.redis.Configuration;
using gigadr3w.msauthflow.sharedcache.redis.Factories;
using gigadr3w.msauthflow.sharedcache.redis.Interfaces;
using Microsoft.Extensions.Options;

namespace gigadr3w.msauthflow.sharedcache.redis
{
    public class RedisSharedCache : ISharedCache
    {
        private readonly IRedisHandler _handler;
        public RedisSharedCache(IOptions<RedisConfiguration> configuration)
            => _handler = RedisHandlerFactory.Create(configuration.Value);

        public Task Delete(string key)
            => _handler.Delete(key);

        public Task<T?> Get<T>(string key)
            => _handler.Get<T>(key);

        public Task Set<T>(string key, T model, TimeSpan? expiry)
            => _handler.Set<T>(key, model, expiry);
    }
}
