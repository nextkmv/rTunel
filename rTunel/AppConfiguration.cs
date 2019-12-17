using System;
using rTunel.ProxyServer;
using static rTunel.ProxyServer.Enums;

namespace rTunel.CoreApp
{
    public class AppConfiguration
    {
        private Credential credential;
        private EndPoints endPoints;

        // -from-Ip 127.0.0.1 -from-port 3001 -to-ip 127.0.0.1 -to-port 3002 -encryption XOR -encryption-private-key 123
        public AppConfiguration(string[] args)
        {
            string fromIp = String.Empty;
            ushort fromPort = 0;
            string toIp = String.Empty;
            ushort toPort = 0;
            Protocols protocol = Protocols.TCP;
            EncryptionMethod encryptionMethod = EncryptionMethod.None;
            string privateKey = String.Empty;

            if(args.Length == 0)
            {
                throw new ArgumentException("No arguments specified");
            }

            if (args.Length % 2 != 0)
            {
                throw new ArgumentException("Not all arguments specified");
            }

            for(byte i = 0; i < args.Length; i++)
            {
                string arg = args[i].ToLower();
                string value = args[i+1].ToLower();
                if (!arg.StartsWith("-"))
                {
                    throw new ArgumentException("Expected Symbol -");
                }
                switch (arg)
                {
                    case "-from-ip": {
                            fromIp = value;
                            break;
                        }
                    case "-from-port":
                        { 
                            ushort.TryParse(value, out fromPort);
                            break;
                        }
                    case "-to-ip":
                        {
                            toIp = value;
                            break;
                        }
                    case "-to-port":
                        {
                            ushort.TryParse(value, out toPort);
                            break;
                        }
                    case "-protocol":
                        {
                            switch (value)
                            {
                                case "tcp":
                                    {
                                        protocol = Protocols.TCP;
                                        break;
                                    }
                                default:
                                    {
                                        throw new ArgumentException("Unknown protocol");
                                    }
                            }
                            break;
                        }
                    case "-encryption":
                        {
                            switch (value)
                            {
                                case "none":
                                    {
                                        encryptionMethod = EncryptionMethod.None;
                                        break;
                                    }
                                case "xor":
                                    {
                                        encryptionMethod = EncryptionMethod.XOR;
                                        break;
                                    }
                                default:
                                    {
                                        throw new ArgumentException("Unknown encryption method");
                                    }
                            }
                            break;
                        }
                    case "-encryption-private-key":
                        {
                            privateKey = value;
                            break;
                        }


                }
                i++;
            }

            credential = new Credential(encryptionMethod, privateKey);

            if (fromIp == String.Empty) throw new ArgumentException("The required argument \"-from-Ip\" is not specified");
            if (fromPort == 0) throw new ArgumentException("The required argument \"-from-Port\" is not specified");
            if (toIp == String.Empty) throw new ArgumentException("The required argument \"-to-Ip\" is not specified");
            if (toPort == 0) throw new ArgumentException("The required argument \"-to-Port\" is not specified");

            endPoints = new EndPoints(fromIp, fromPort, toIp, toPort, protocol);

        }

        public EndPoints GetEndPoints()
        {
            return endPoints;
        }

        public Credential GetCredential()
        {
            return credential;
        }

    }
}
