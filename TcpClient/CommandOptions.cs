using CommandLine;

namespace TcpClient
{
    internal class CommandOptions
    {
        [Option("ip", Required = true, HelpText = "Please set ip address.")]
        public string? IpAddress { get; set; }

        [Option('m', Required = true, HelpText = "Please set send message to server.")]
        public string? Message { get; set; }

        [Option('p', Required = true, HelpText = "Please set tcp port number.")]
        public int Port { get; set; }
    }
}
