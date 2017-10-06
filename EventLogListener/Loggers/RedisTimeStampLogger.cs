
using System;
using System.Diagnostics;
using StackExchange.Redis;
using System.Collections.Generic;

namespace EventLogListener.Loggers
{
    public class RedisTimeStampLogger : RedisEventLoggerBase
    {
        private IEnumerable<int> _keyReplacementStringIndexes { get; set; }

        /// <summary>
        /// This logger will will set key(keyReplacementStringIndex) as value(valueReplacementStringIndex)
        /// </summary>
        /// <param name="redisConnectionString">REDIS server connection string</param>
        /// <param name="keyReplacementStringIndex">Index of the Event's ReplacementString that will be used as key</param>
        /// <param name="evt_id">If greater than 0, will log only events which EventID match the given evt_id</param>
        /// <param name="async">do not wait for REDIS confirmation of SET operation</param>
        /// <param name="expiration">REDIS key x value pair expiration in seconds</param>
        /// <returns>RedisEventLogger instance</returns>
        public RedisTimeStampLogger(string redisConnectionString, IEnumerable<int> keyReplacementStringIndexes, int evt_id = 0, bool async = true, int? expiration = null) : base(redisConnectionString, async, expiration)
        {
            _keyReplacementStringIndexes = keyReplacementStringIndexes;
        }

        private long toUnixTime(DateTime dt)
        {
            var timeSpan = (dt - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        public override void log(EntryWrittenEventArgs e)
        {
            if (_eventId <= 0 || e.Entry.InstanceId == _eventId)
            {
                var db = _redis.GetDatabase();
                var keys = new List<string>();
                foreach (int index in _keyReplacementStringIndexes)
                {
                    keys.Add(e.Entry.ReplacementStrings[index]);
                }
                var key = string.Join("-", keys);
                var value = toUnixTime(e.Entry.TimeGenerated).ToString();
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