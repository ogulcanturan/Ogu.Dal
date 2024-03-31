using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;

namespace Ogu.Dal.Redis.Cache
{
    public class DistributedRedisCache : RedisCache, IDistributedRedisCache
    {
        public DistributedRedisCache(IOptions<RedisCacheOptions> optionsAccessor) : base(optionsAccessor) { }
    }
}