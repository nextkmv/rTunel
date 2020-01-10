using rTunel.ProxyServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rTunel.WinowsService
{
    public partial class rTunel : ServiceBase
    {
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        private Server[] servers;

        public rTunel()
        {
            InitializeComponent();
            Configuration configuration = ServiceConfiguration.GetConfiguration(true);
            servers = new Server[configuration.Preferences.Length];
            for (byte i = 0; i < configuration.Preferences.Length; i++)
            {
                servers[i] = new Server(configuration.Preferences[i].EndPoints, configuration.Preferences[i].Credential);
            }
        }


        private void StartProxy()
        {
            Log.Debug($"Start proxy services");
            Configuration configuration = ServiceConfiguration.GetConfiguration();
            if (configuration.Preferences.Length == 0)
            {
                Log.Info("No preference from loading conf. Service is stoped.");
                Stop();
            }

            Task[] tasks = new Task[configuration.Preferences.Length];
            for (byte i = 0; i < configuration.Preferences.Length; i++)
            {
                Log.Debug($"Starting proxy {servers[i].endPoints}");

                try
                {
                    tasks[i] = servers[i].Start();
                }
                catch (Exception e)
                {
                    Log.Fatal(e, $"Dont starting proxy {servers[i].endPoints}");
                }

            }
            
            Task.WhenAny(tasks).Wait();
        }

        public void Start()
        {
            Log.Info("Attempt to start a service");
            Thread serviceThread = new Thread(new ThreadStart(StartProxy));
            serviceThread.Start();
        }

        protected override void OnStart(string[] args)
        {
            Start();
        }

        protected override void OnStop()
        {
            foreach(Server server in servers)
            {
                Log.Info("Stoping all services");
                server.Stop();
            }
        }
    }
}
