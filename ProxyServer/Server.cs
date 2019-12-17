using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace rTunel.ProxyServer
{
    public class Server : IProxy
    {
        EndPoints endPoints;
        Credential credential;

        public Server(EndPoints endPoints, Credential credential)
        {
            this.endPoints = endPoints;
            this.credential = credential;
        }

        public async Task Start()
        {
            IPAddress localIpAddress = string.IsNullOrEmpty(endPoints.FromIp) ? IPAddress.IPv6Any : IPAddress.Parse(endPoints.FromIp);
            var server = new System.Net.Sockets.TcpListener(new IPEndPoint(localIpAddress, endPoints.FromPort));
            server.Start();
            Console.WriteLine($"TCP proxy started {endPoints.FromIp}:{endPoints.FromPort}...");

            while (true)
            {
                try
                {
                    var remoteClient = await server.AcceptTcpClientAsync();
                    remoteClient.NoDelay = true;
                    var ips = await Dns.GetHostAddressesAsync(endPoints.ToIp);
                    new TcpClient(remoteClient, new IPEndPoint(ips.First(), endPoints.ToPort),credential);

                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex);
                    Console.ResetColor();
                }
            }
        }
    }


    class TcpClient
    {
        private System.Net.Sockets.TcpClient remoteClient;
        private IPEndPoint clientEndpoint;
        private IPEndPoint remoteServer;
        Credential credential;

        public TcpClient(System.Net.Sockets.TcpClient remoteClient, IPEndPoint remoteServer, Credential credential)
        {
            this.remoteClient = remoteClient;
            this.remoteServer = remoteServer;
            this.credential = credential;
            client.NoDelay = true;
            this.clientEndpoint = (IPEndPoint)this.remoteClient.Client.RemoteEndPoint;
            Console.WriteLine($"Established {this.clientEndpoint} => {this.remoteServer}");
            Run();
        }


        public System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();

        public async Task CopyToAsync(Stream source, Stream destination)
        {
            Int32 bufferSize = 81920;
            CancellationToken cancellationToken = CancellationToken.None;
            byte[] buffer = new byte[bufferSize];
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                buffer = new Encryption(credential).GetEncryptionData(buffer);
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
            }
        }

        private void Run()
        {

            _ = Task.Run(async () =>
              {
                  try
                  {
                      using (this.remoteClient)
                      using (client)
                      {
                          await client.ConnectAsync(this.remoteServer.Address, this.remoteServer.Port);
                          var serverStream = client.GetStream();
                          var remoteStream = this.remoteClient.GetStream();

                          await Task.WhenAny(CopyToAsync(remoteStream,serverStream), CopyToAsync(serverStream,remoteStream));

                      }
                  }
                  catch (Exception e)
                  {
                      Console.WriteLine(e.ToString());
                  }
                  finally
                  {
                      Console.WriteLine($"Closed {this.clientEndpoint} => {this.remoteServer}");
                      this.remoteClient = null;
                  }
              });
        }


    }

}
