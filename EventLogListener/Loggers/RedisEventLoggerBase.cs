
using System;
using System.Diagnostics;
using StackExchange.Redis;
using System.IO;

namespace EventLogListener.Loggers
{
    public abstract class RedisEventLoggerBase : IEventLogger
    {
        protected ConnectionMultiplexer _redis;
        protected bool _async;
        protected int? _expiration;
        protected int _eventId;

        /// <summary>
        /// This is the base class which Redis Loggers should inherit
        /// </summary>
        /// <param name="redisConnectionString">REDIS server connection string</param>
        /// <param name="async">do not wait for REDIS confirmation of SET operation</param>
        /// <param name="expiration">REDIS key x value pair expiration in seconds</param>
        /// <returns>RedisEventLogger instance</returns>
        protected RedisEventLoggerBase(string redisConnectionString, bool async = true, int? expiration = null)
        {
            ConfigurationOptions config = ConfigurationOptions.Parse(redisConnectionString);
            config.AbortOnConnectFail = false;
            config.AllowAdmin = false;
            config.ConnectRetry = 3;
            config.KeepAlive = 180;
            StringWriter sw = new StringWriter();
            _redis = ConnectionMultiplexer.Connect(config,sw);
            _expiration = expiration;
            _async = async;
        }

        public abstract void log(EntryWrittenEventArgs e);
    }
}