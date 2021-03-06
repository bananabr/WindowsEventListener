﻿
using System;
using System.Diagnostics;
using StackExchange.Redis;
using System.IO;

namespace EventLogListener.Loggers
{
    public class RedisReplacementStringLogger:RedisEventLoggerBase
    {
        private int _keyIndex;
        private int _valueIndex;

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
        public RedisReplacementStringLogger(string redisConnectionString, int keyReplacementStringIndex, int valueReplacementStringIndex, int evt_id = 0, bool async = true, int? expiration = null) : base(redisConnectionString, async, expiration)
        {
            _keyIndex = keyReplacementStringIndex;
            _valueIndex = valueReplacementStringIndex;
            _eventId = evt_id;
        }

        public override void log(EntryWrittenEventArgs e)
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