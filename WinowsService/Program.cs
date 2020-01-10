using NLog;
using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;


namespace rTunel.WinowsService
{
    static class Program
    {

        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        private static void InitLog()
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") {
                FileName = "${basedir}/logs/${shortdate}.log",
                Layout = "${longdate} ${uppercase:${level}} ${message}"
            };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // Apply config           
            NLog.LogManager.Configuration = config;
        }

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main(string[] args)
        {
            InitLog();

            if (System.Environment.UserInteractive)
            {
                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                }
            }
            else
            {
                Log.Info("Start as service");
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                            new rTunel()
                };
                ServiceBase.Run(ServicesToRun);
            }


            

        }
    }
}
