using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using rTunel.ProxyServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;




namespace rTunel.WinowsService
{

    public class Preference
    {
        public string Name;
        public Credential Credential;
        public EndPoints EndPoints;
    }

    public class Configuration
    {
        public string Name;
        public Preference[] Preferences;
    }

    public static class ServiceConfiguration
    {
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        private static Configuration configuration = null;

        private static Configuration getConfigurationFromJSON()
        {
            const string fileName = "config.json";
            string path = Path.Combine(Environment.CurrentDirectory, fileName);
            if(!File.Exists(path))
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            }
            if (!File.Exists(path))
            {
                path = Path.Combine(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), fileName);
            }

            if (!File.Exists(path))
            {
                Log.Info("Config file not found. Loading default configuration");
                return getDefaultConfiguration();
            }
            else
            {
                Log.Debug($"Get config from path {path}");
                string JSON = File.ReadAllText(path);
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = new List<JsonConverter> { new StringEnumConverter { CamelCaseText = true } },
                    NullValueHandling = NullValueHandling.Ignore
                };
                return JsonConvert.DeserializeObject<Configuration>(JSON, jsonSerializerSettings);
            }
        }

        public static Configuration getDefaultConfiguration()
        {
            Configuration config = new Configuration();
            config.Name = "Default config";
            config.Preferences =new Preference[1];
            config.Preferences[0] = new Preference();
            config.Preferences[0].Name = "Default rule";
            config.Preferences[0].EndPoints = new EndPoints("127.0.0.1", 8080, "127.0.0.1", 8080, Enums.Protocols.TCP);
            config.Preferences[0].Credential = new Credential(Enums.EncryptionMethod.None, String.Empty);
            Log.Debug($"Get default conf");
            return config;
        }

        public static string getDefaultJSON()
        {
            Configuration config = getDefaultConfiguration();
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new StringEnumConverter { CamelCaseText = true } },
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(config, jsonSerializerSettings);
        }


        public static Configuration GetConfiguration(bool update = false)
        {
            if(configuration != null && update == false)
            {
                Log.Debug($"Get loading conf");
                return configuration;
            }
            else
            {
                configuration = getConfigurationFromJSON();
                Log.Debug($"Load conf");
                return configuration;
            }
        }



    }
}
