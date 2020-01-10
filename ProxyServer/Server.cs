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
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
        private bool running = false;

        public bool IsRunning { get { return running; } }

        public EndPoints endPoints;
        public Credential credential;

        public Server(EndPoints endPoints, Credential credential)
        {
            this.endPoints = endPoints;
            this.credential = credential;
        }

        public void Stop()
        {
            running = false;
        }

        public async Task Start()
        {
            IPAddress localIpAddress = string.IsNullOrEmpty(endPoints.FromIp) ? IPAddress.IPv6Any : IPAddress.Parse(endPoints.FromIp);
            var server = new System.Net.Sockets.TcpListener(new IPEndPoint(localIpAddress, endPoints.FromPort));
            server.Start();
            Log.Info($"TCP proxy started on {endPoints}. {credential.ToString()}");
            running = true;
            while (running)
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
                    Log.Debug(ex, $"Start server on {endPoints} exception: {ex.ToString()}");
                }
            }
        }
    }


    class TcpClient
    {
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
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
            Log.Debug($"Established {this.clientEndpoint} => {this.remoteServer}");
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
                      Log.Error(e, $"Run server exception: {e.ToString()}");
                  }
                  finally
                  {
                      Log.Debug($"Closed {this.clientEndpoint} => {this.remoteServer}");
                      this.remoteClient = null;
                  }
              });
        }


    }

}
