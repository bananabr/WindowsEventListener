using EventLogListener;
using EventLogListener.Filters;
using EventLogListener.Loggers;
using LoggingUtils;
using System;
using System.Collections.Generic;
using System.Security;
using System.Threading;

namespace WindowsEventLogger
{
    class Program
    {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);
        static string redisConnectionString, domain, logName;
        static int redisTTL;
        static ILogger logger = new ConsoleLogger();
        static EventLogHandler handler;
        static int remote_network_address_index;
        static int username_index;

        static void Init()
        {
            logName = "Security";
            redisConnectionString = Properties.Settings.Default["RedisServers"].ToString();
            domain = Properties.Settings.Default["WindowsDomainRegex"].ToString();
            int.TryParse(Properties.Settings.Default["RedisTTL"].ToString(),out redisTTL);
            var os_version = Environment.OSVersion.Version;
            handler = NetworkLogonEventsHandlerFactory.Build(os_version.Major,domain);
            IEventLogger consoleLogger = new ConsoleReplacementStringsLogger();
            if (os_version.Major > 5)
            {
                remote_network_address_index = 18;
                username_index = 5;
            }
            else
            {
                remote_network_address_index = 13;
                username_index = 0;
            }
            IEventLogger redisLogger = new RedisEventLogger(redisConnectionString, remote_network_address_index, username_index, false, redisTTL);
            IEventFilter usernameFilter = new NOT_EventFilter(new ReplacementStringFilter(new Dictionary<int, string>() { { username_index, @"^.*\$$" } }));
            handler.RegisterFilter(usernameFilter);
            handler.RegisterLogger(consoleLogger);
            handler.RegisterLogger(redisLogger);
            handler.SetExceptionLogger(logger);
        }

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };
            
            try
            {
                Console.WriteLine(String.Format(@"Identified OS version is {0}.", Environment.OSVersion.VersionString));
                Init();
                Console.WriteLine(@"Finished loading configuration.");
                WindowsEventLogListener listener = new WindowsEventLogListener(logName, handler);
                Console.WriteLine(@"Listenning to events ...");
            }
            catch (SecurityException)
            {
                logger.Log(LogLevels.Error, String.Format(@"Permission denied when trying to access the '{0}' log.", logName));
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                logger.Log(LogLevels.Error, String.Format("Something bad hapenned, do something about it!\nException details:\n{0}", e.Message));
                Environment.Exit(1);
            }

        _quitEvent.WaitOne();
            Environment.Exit(0);
        }
    }
}