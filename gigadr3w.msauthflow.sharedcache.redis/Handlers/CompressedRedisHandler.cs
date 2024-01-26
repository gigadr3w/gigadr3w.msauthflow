using gigadr3w.msauthflow.sharedcache.redis.Interfaces;
using static gigadr3w.msauthflow.common.Handlers.CompressionHandler;
using StackExchange.Redis;
using System.Text.Json;

namespace gigadr3w.msauthflow.sharedcache.redis.Handlers
{
    internal class CompressedRedisHandler : IRedisHandler
    {
        private readonly IDatabase _database;

        public CompressedRedisHandler(IDatabase database)
            => _database = database;

        public Task Delete(string key)
            => _database.KeyDeleteAsync(key);

        public async Task<T?> Get<T>(string key)
        {
            byte[] compressed = await _database.StringGetAsync(key);
            if (compressed == null) return default;

            string json = Decompress(compressed);
            if (string.IsNullOrEmpty(json)) return default;

            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task Set<T>(string key, T model, TimeSpan? expiry)
        {
            string json = JsonSerializer.Serialize(model);
            byte[] compressed = Compress(json);

            await _database.StringSetAsync(key, compressed, expiry: expiry);
        }
    }
}
