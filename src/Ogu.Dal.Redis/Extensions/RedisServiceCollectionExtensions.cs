using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Ogu.Dal.Redis.Cache;
using Ogu.Dal.Redis.DataContext;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ogu.Dal.Redis.Extensions
{
    public static class RedisServiceCollectionExtensions
    {
        internal static Task<TContext> InitializeRedisAsync<TContext>(ConfigurationOptions configurationOptions, RedisContextOptions redisContextOptions) where TContext : RedisContext
        {
            var context = (TContext)Activator.CreateInstance(typeof(TContext), configurationOptions, redisContextOptions);
            return context.ConnectAsync().ContinueWith((t) => context);
        }

        public static IServiceCollection AddRedisContext<TContext>(this IServiceCollection services,
            string configuration, bool ignoreUnknown = true, Action<RedisContextOptions> redisContextOpts = null) where TContext : RedisContext
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            var configurationOptions = ConfigurationOptions.Parse(configuration, ignoreUnknown);

            var redisContextOptions = new RedisContextOptions();

            redisContextOpts?.Invoke(redisContextOptions);

            services.AddSingleton(configurationOptions);
            services.AddSingleton(redisContextOptions);
            services.AddSingleton<TContext>();

            return services;
        }

        /// <summary>
        /// Service can be resolve from TContext
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="configurationOpts"></param>
        /// <param name="redisContextOpts"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddRedisContext<TContext>(this IServiceCollection services, Action<ConfigurationOptions> configurationOpts, Action<RedisContextOptions> redisContextOpts = null) where TContext : RedisContext
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            configurationOpts = configurationOpts ?? throw new ArgumentNullException(nameof(configurationOpts));

            var configurationOptions = new ConfigurationOptions();
            var redisContextOptions = new RedisContextOptions();

            configurationOpts.Invoke(configurationOptions);
            redisContextOpts?.Invoke(redisContextOptions);

            services.AddSingleton(configurationOptions);
            services.AddSingleton(redisContextOptions);
            services.AddSingleton<TContext>();

            return services;
        }

        /// <summary>
        /// Service can be resolved from Task TContext
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="ignoreUnknown"></param>
        /// <param name="redisContextOpts"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddAsyncRedisContext<TContext>(this IServiceCollection services,
            string configuration, bool ignoreUnknown = true, Action<RedisContextOptions> redisContextOpts = null) where TContext : RedisContext
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            var configurationOptions = ConfigurationOptions.Parse(configuration, ignoreUnknown);

            var redisContextOptions = new RedisContextOptions();

            redisContextOpts?.Invoke(redisContextOptions);

            services.AddSingleton(s => InitializeRedisAsync<TContext>(configurationOptions, redisContextOptions));

            return services;
        }

        /// <summary>
        /// Service can be resolved from Task TContext
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="configurationOpts"></param>
        /// <param name="redisContextOpts"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddAsyncRedisContext<TContext>(this IServiceCollection services, Action<ConfigurationOptions> configurationOpts, Action<RedisContextOptions> redisContextOpts = null) where TContext : RedisContext
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            configurationOpts = configurationOpts ?? throw new ArgumentNullException(nameof(configurationOpts));

            var configurationOptions = new ConfigurationOptions();
            var redisContextOptions = new RedisContextOptions();

            configurationOpts.Invoke(configurationOptions);
            redisContextOpts?.Invoke(redisContextOptions);

            services.AddSingleton(s => InitializeRedisAsync<TContext>(configurationOptions, redisContextOptions));

            return services;
        }

        /// <summary>
        /// Service can be resolved from `IDistributedRedisCache` interface
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddDistributedRedisCache(this IServiceCollection services, Action<RedisCacheOptions> setupAction)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            setupAction = setupAction ?? throw new ArgumentNullException(nameof(setupAction));

            services.AddOptions();
            services.Configure(setupAction);

            services.Add(ServiceDescriptor.Singleton<IDistributedRedisCache, DistributedRedisCache>());
            return services;
        }
    }
}