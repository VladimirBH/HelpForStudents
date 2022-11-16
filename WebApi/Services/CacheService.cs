using Microsoft.Extensions.Caching.Memory;

namespace WebApi.Services

{
    public class CacheService : ICacheService
    {
        MemoryCache cache  = new MemoryCache(new MemoryCacheOptions());
        public CacheService()
        {

        }

        public void SetCodeForConfirmationEmail(string key, string code)
        {
            cache.Set(key, code, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }

        public string GetCodeForConfirmationEmail(string key)
        {
            string code;
            if(cache.TryGetValue<string>(key, out code))
            {
                return code;
            }
            return string.Empty;
        }

        public void DeleteFromCache(string key)
        {
            cache.Remove(key); 
        }
    }
}