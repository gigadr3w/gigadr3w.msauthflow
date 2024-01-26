namespace gigadr3w.msauthflow.sharedcache.Interfaces
{
    /// <summary>
    /// Cache shared between microservices
    /// </summary>
    public interface ISharedCache
    {
        Task<T?> Get<T>(string key);
        Task Set<T>(string key, T model, TimeSpan? expiry);
        Task Delete(string key);
    }
}
