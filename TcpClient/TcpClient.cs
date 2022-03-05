using System.Net.Sockets;
using System.Net;
using System.Text;

namespace TcpClient
{
    internal class TcpClient
    {
        private static readonly ManualResetEvent ConnectDone = new(false);
        private static readonly ManualResetEvent SendDone = new(false);
        private static readonly ManualResetEvent ReceiveDone = new(false);
        private static string Response = string.Empty;

        public static string StartClient(string ipAddress, int port, string? data)
        {
            Response = string.Empty;

            // Reset signal
            ConnectDone.Reset();
            SendDone.Reset();
            ReceiveDone.Reset();

            // Connect server
            try
            {
                var ip = IPAddress.Parse(ipAddress == "localhost" ? "127.0.0.1" : ipAddress);
                var endPoint = new IPEndPoint(ip, port);
                var client = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Start connect
                client.BeginConnect(endPoint, ConnectCallback, client);
                ConnectDone.WaitOne();

                var byteData = Encoding.ASCII.GetBytes(data + "<EOF>");
                client.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, client);
                SendDone.WaitOne();

                var state = new StateObject
                {
                    WorkSocket = client
                };

                client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                ReceiveDone.WaitOne();

                client.Shutdown(SocketShutdown.Both);
                client.Close();

                Console.WriteLine("Finish connection.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return Response;
        }

        private static void ConnectCallback(IAsyncResult result)
        {
            try
            {
                if (result.AsyncState is not Socket client)
                {
                    Console.WriteLine("Tcp client is not found.");
                    return;
                }
                client.EndConnect(result);
                Console.WriteLine("Finished connect.");

                ConnectDone.Set();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void SendCallback(IAsyncResult result)
        {
            try
            {
                if (result.AsyncState is not Socket client)
                {
                    Console.WriteLine("Tcp client is not found.");
                    return;
                }

                var bytesSend = client.EndSend(result);
                Console.WriteLine($"Send complete. Send size: {bytesSend}");

                SendDone.Set();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                if (result.AsyncState is not StateObject state)
                {
                    Console.WriteLine("StateObject is not found.");
                    return;
                }

                var client = state.WorkSocket;
                if (client == null)
                {
                    return;
                }
                var bytesRead = client.EndReceive(result);
                if (bytesRead > 0)
                {
                    state.Sb.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
                    client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                }
                else
                {
                    if (state.Sb.Length > 1)
                    {
                        Response = state.Sb.ToString();
                        Console.WriteLine($"Receive message from server. Message: {Response}");
                    }

                    ReceiveDone.Set();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
