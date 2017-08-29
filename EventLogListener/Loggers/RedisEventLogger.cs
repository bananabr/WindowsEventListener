
using System;
using System.Diagnostics;
using StackExchange.Redis;
using System.IO;

namespace EventLogListener.Loggers
{
    public class RedisEventLogger:IEventLogger
    {
        private ConnectionMultiplexer _redis;
        private int _keyIndex;
        private int _valueIndex;
        private bool _async;
        private int? _expiration;
        private int _eventId;

        /// <summary>
        /// This logger will will set key(keyReplacementStringIndex) as value(valueReplacementStringIndex)
        /// </summary>
        /// <param name="redisConnectionString">REDIS server connection string</param>
        /// <param name="keyReplacementStringIndex">Index of the Event's ReplacementString that will be used as key</param>
        /// <param name="valueReplacementStringIndex">Index of the Event's ReplacementString which value will be stored at key</param>
        /// <param name="evt_id">If greater than 0, will log only events which EventID match the given evt_id</param>
        /// <param name="async">do not wait for REDIS confirmation of SET operation</param>
        /// <param name="expiration">REDIS key x value pair expiration in seconds</param>
        /// <returns>RedisEventLogger instance</returns>
        public RedisEventLogger(string redisConnectionString, int keyReplacementStringIndex, int valueReplacementStringIndex, int evt_id = 0, bool async = true, int? expiration = null)
        {
            ConfigurationOptions config = ConfigurationOptions.Parse(redisConnectionString);
            config.AbortOnConnectFail = false;
            config.AllowAdmin = false;
            config.ConnectRetry = 3;
            config.KeepAlive = 180;
            StringWriter sw = new StringWriter();
            _redis = ConnectionMultiplexer.Connect(config,sw);
            _keyIndex = keyReplacementStringIndex;
            _valueIndex = valueReplacementStringIndex;
            _expiration = expiration;
            _async = async;
            _eventId = evt_id;
        }

        public void log(EntryWrittenEventArgs e)
        {
            if (_eventId <= 0 || e.Entry.InstanceId == _eventId)
            {
                var db = _redis.GetDatabase();
                var key = e.Entry.ReplacementStrings[_keyIndex];
                var value = e.Entry.ReplacementStrings[_valueIndex];
                var flags = _async ? CommandFlags.FireAndForget : CommandFlags.None;
                TimeSpan? exp;
                if (_expiration != null)
                    exp = new TimeSpan(0, 0, _expiration.Value);
                else
                    exp = null;
                db.StringSet(key, value, exp, When.Always, flags);
            }
        }
    }
}