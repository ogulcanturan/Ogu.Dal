using Ogu.Dal.Redis.DataContext;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ogu.Dal.Redis.Extensions
{
    public static class RedisExtensions
    {
        public static TContext CreateRedisContext<TContext>(string configuration, bool ignoreUnknown = true) where TContext : RedisContext
        {
            var configurationOptions = ConfigurationOptions.Parse(configuration, ignoreUnknown);
            var context = (TContext)Activator.CreateInstance(typeof(TContext), configurationOptions);
            return context;
        }

        public static TContext CreateRedisContext<TContext>(Action<ConfigurationOptions> configurationOpts, Action<RedisContextOptions> redisContextOpts = null) where TContext : RedisContext
        {
            var configurationOptions = new ConfigurationOptions();
            var redisContextOptions = new RedisContextOptions();
            configurationOpts?.Invoke(configurationOptions);
            redisContextOpts?.Invoke(redisContextOptions);
            var context = (TContext)Activator.CreateInstance(typeof(TContext), configurationOptions, redisContextOptions);
            return context;
        }

        public static async Task<TContext> CreateAsyncRedisContext<TContext>(string configuration, bool ignoreUnknown = true, Action<RedisContextOptions> redisContextOpts = null) where TContext : RedisContext
        {
            var configurationOptions = ConfigurationOptions.Parse(configuration, ignoreUnknown);
            var redisContextOptions = new RedisContextOptions();
            redisContextOpts?.Invoke(redisContextOptions);
            return await RedisServiceCollectionExtensions.InitializeRedisAsync<TContext>(configurationOptions, redisContextOptions);
        }

        public static async Task<TContext> CreateAsyncRedisContext<TContext>(Action<ConfigurationOptions> configurationOpts, Action<RedisContextOptions> redisContextOpts = null) where TContext : RedisContext
        {
            var configurationOptions = new ConfigurationOptions();
            var redisContextOptions = new RedisContextOptions();
            configurationOpts?.Invoke(configurationOptions);
            redisContextOpts?.Invoke(redisContextOptions);
            return await RedisServiceCollectionExtensions.InitializeRedisAsync<TContext>(configurationOptions, redisContextOptions);
        }
    }
}