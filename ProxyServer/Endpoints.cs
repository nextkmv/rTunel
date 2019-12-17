using System;
using static rTunel.ProxyServer.Enums;

namespace rTunel.ProxyServer
{
    public class EndPoints
    {
        private string fromIp;
        private ushort fromPort;
        private string toIp;
        private ushort toPort;
        private Protocols protocol;

        public string FromIp { get => fromIp; }
        public ushort FromPort { get => fromPort; }
        public string ToIp { get => toIp; }
        public ushort ToPort { get => toPort; }
        public Protocols Protocol { get => protocol; }

        public EndPoints(string fromIp, ushort fromPort, string toIp, ushort toPort, Protocols protocol)
        {       
            this.fromIp = fromIp;
            this.fromPort = fromPort;
            this.toIp = toIp;
            this.toPort = toPort;
            this.protocol = protocol;
        }

    }
}
