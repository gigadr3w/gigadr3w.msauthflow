namespace gigadr3w.msauthflow.sharedcache.redis.Interfaces
{
    internal interface IRedisHandler
    {
        Task<T?> Get<T>(string key);
        Task Set<T>(string key, T model, TimeSpan? expiry);
        Task Delete(string key);
    }
}
