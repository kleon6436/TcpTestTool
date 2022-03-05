using System.Net.Sockets;
using System.Net;
using System.Text;

namespace TcpServer
{
    internal class TcpServer
    {
        public static async Task<bool> StartListening(int port)
        {
            // Set endpoint to wait connection.
            var localEndPoint = new IPEndPoint(IPAddress.Any, port);

            // Create TcpListener
            var tcpServer = new TcpListener(localEndPoint);

            try
            {
                tcpServer.Start();

                while (true)
                {
                    Console.WriteLine("Wait tcp connection...");

                    using var tcpClient = await tcpServer.AcceptTcpClientAsync();
                    Console.WriteLine("Accept to connect from tcp client.");

                    await ReceiveAsync(tcpClient);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }

        private static async Task ReceiveAsync(TcpClient tcpClient)
        {
            var buffer = new byte[1024];
            var request = "";

            try
            {
                await using var stream = tcpClient.GetStream();
                do
                {
                    var byteSize = await stream.ReadAsync(buffer);
                    request += Encoding.UTF8.GetString(buffer, 0, byteSize);
                } while (stream.DataAvailable);

                Console.WriteLine($"Receive message. Message:{request}");

                const string Response = "OK";
                buffer = Encoding.ASCII.GetBytes(Response);

                await stream.WriteAsync(buffer);
                Console.WriteLine($"Send message. Message:{Response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
