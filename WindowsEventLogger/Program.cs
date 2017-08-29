using EventLogListener;
using EventLogListener.Filters;
using EventLogListener.Loggers;
using ip2userLib;
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
        static EventLogHandler kerberosEventsHandler;
        //static EventLogHandler handler;
        static int remote_network_address_index, krb_client_addr_index, username_index;

        static void Init()
        {
            logName = "Security";
            redisConnectionString = Properties.Settings.Default["RedisServers"].ToString();
            domain = Properties.Settings.Default["WindowsDomainRegex"].ToString();
            int.TryParse(Properties.Settings.Default["RedisTTL"].ToString(),out redisTTL);
            var os_version = Environment.OSVersion.Version;
            IEventLogger consoleLogger = new ConsoleReplacementStringsLogger();
            if (os_version.Major > 5)
            {
                remote_network_address_index = 18;
                username_index = 5;
            }
            else
            {
                remote_network_address_index = 13;
                krb_client_addr_index = 6;
                username_index = 0;
            }
            //IEventLogger redisLogger = new RedisEventLogger(redisConnectionString, remote_network_address_index, username_index, false, redisTTL);
            IEventFilter usernameFilter = new NOT_EventFilter(new ReplacementStringFilter(new Dictionary<int, string>() { { username_index, @"^.*\$.*$" } }));

            /*handler = NetworkLogonEventsHandlerFactory.Build(os_version.Major, domain);
            handler.RegisterFilter(usernameFilter);
            handler.RegisterLogger(consoleLogger);
            handler.RegisterLogger(redisLogger);
            handler.SetExceptionLogger(logger);*/


            //Kerberos Ticket Request Event Handler
            kerberosEventsHandler = NetworkLogonEventsHandlerFactory.Build(os_version.Major, domain, NetworkLogonEventSources.KERBEROS);
            kerberosEventsHandler.RegisterFilter(usernameFilter);
            kerberosEventsHandler.RegisterLogger(consoleLogger);

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
                WindowsEventLogListener listener = new WindowsEventLogListener(logName, kerberosEventsHandler);
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