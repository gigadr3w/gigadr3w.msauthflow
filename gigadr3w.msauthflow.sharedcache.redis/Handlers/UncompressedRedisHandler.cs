using gigadr3w.msauthflow.sharedcache.redis.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace gigadr3w.msauthflow.sharedcache.redis.Handlers
{
    internal class UncompressedRedisHandler : IRedisHandler
    {
        private readonly IDatabase _database;

        internal UncompressedRedisHandler(IDatabase database)
            => _database = database;

        Task IRedisHandler.Delete(string key)
            => _database.KeyDeleteAsync(key);

        async Task<T?> IRedisHandler.Get<T>(string key) where T : default
        {
            string json = await _database.StringGetAsync(key);
            return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
        }

        async Task IRedisHandler.Set<T>(string key, T model, TimeSpan? expiry)
        {
            string json = JsonSerializer.Serialize(model);
            await _database.StringSetAsync(key, json, expiry: expiry);
        }
    }
}
