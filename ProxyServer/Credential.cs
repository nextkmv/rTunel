using System;
using System.Collections.Generic;
using System.Text;
using rTunel.ProxyServer;
using static rTunel.ProxyServer.Enums;

namespace rTunel.ProxyServer
{
    public class Credential
    {
        private EncryptionMethod method = EncryptionMethod.None;
        private string privateKey = "";

        public EncryptionMethod Method { get => method; }
        public string PrivateKey { get => privateKey; }

        public Credential(EncryptionMethod method, string privateKey)
        {
            this.method = method;
            this.privateKey = privateKey;
        }

        public override string ToString()
        {
            string isPrivateKey = "No";
            if(privateKey != String.Empty)
            {
                isPrivateKey = "YES";
            }

            return $"Credential method {method}, use key {isPrivateKey}";
        }
    }
}
