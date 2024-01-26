using gigadr3w.msauthflow.sharedcache.redis.Configuration;
using gigadr3w.msauthflow.sharedcache.redis.Handlers;
using gigadr3w.msauthflow.sharedcache.redis.Interfaces;
using StackExchange.Redis;

namespace gigadr3w.msauthflow.sharedcache.redis.Factories
{
    internal static class RedisHandlerFactory
    {
        internal static IRedisHandler Create(RedisConfiguration configuration)
        {
            if(string.IsNullOrEmpty(configuration.ConnectionString)) throw new ArgumentNullException($"{nameof(configuration.ConnectionString)} is mandatory");

            ConnectionMultiplexer mutiplexer = ConnectionMultiplexer.Connect(configuration.ConnectionString);
            IDatabase database = mutiplexer.GetDatabase();

            if (configuration.EnableCompression)
            {
                return new CompressedRedisHandler(database);
            }
            else
            {
                return new UncompressedRedisHandler(database);
            }
        }
    }
}
