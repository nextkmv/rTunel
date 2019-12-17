using System;
using static rTunel.ProxyServer.Enums;

namespace rTunel.ProxyServer
{
    public class Encryption
    {
        Credential credential;

        public Credential Credential { get => credential; }

        public Encryption(Credential credential)
        {
            this.credential = credential;
        }

        public byte[] GetEncryptionData(byte[] inputData)
        {
            switch (credential.Method)
            {
                case EncryptionMethod.XOR:
                {
                    return GetXOREncryptionData(inputData);
                }
                default: return inputData;
            }
            
        }


        private byte[] GetXOREncryptionData(byte[] data)
        {
            string key = credential.PrivateKey;
            if (key.Length == 0) return data;
            byte[] keyByte = System.Text.Encoding.UTF8.GetBytes(key);

            byte[] result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                int pos = i % key.Length / data.Length;
                result[i] = (byte)(data[i] ^ key[i % key.Length / data.Length]);
            }

            return result;
        }

    }
}
