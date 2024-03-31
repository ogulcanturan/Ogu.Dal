using System;

namespace Ogu.Dal.Redis
{
    public class RedisContextOptions
    {
        public TimeSpan ReconnectMinInterval { get; set; } = TimeSpan.FromSeconds(60);
        public TimeSpan ReconnectErrorThreshold { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan RestartConnectionTimeout { get; set; } = TimeSpan.FromSeconds(15);
        public int? RetryMaxAttempts { get; set; } = 5;
    }
}