using CommandLine;

namespace TcpServer
{
    internal class CommandOptions
    {
        [Option('p', Required = true, HelpText = "Please set tcp port number.")]
        public int Port { get; set; }
    }
}
