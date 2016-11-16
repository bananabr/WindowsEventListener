
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

        public RedisEventLogger(string redisConnectionString, int keyReplacementStringIndex, int valueReplacementStringIndex, bool async = true, int? expiration = null)
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
        }

        public void log(EntryWrittenEventArgs e)
        {
            var db = _redis.GetDatabase();
            var key = e.Entry.ReplacementStrings[_keyIndex];
            var value = e.Entry.ReplacementStrings[_valueIndex];
            var flags = _async ? CommandFlags.FireAndForget : CommandFlags.None;
            TimeSpan? exp;
            if (_expiration != null)
                exp = new TimeSpan(0,0,_expiration.Value);
            else
                exp = null;
            db.StringSet(key,value,exp,When.Always,flags);
        }
    }
}