using System.Net.Sockets;
using System.Text;

namespace TcpClient
{
    public class StateObject
    {
        public const int BufferSize = 1024;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder Sb = new();
        public Socket? WorkSocket = null;
    }
}
