using CommandLine;
using CommandLine.Text;

namespace TcpClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Parse commandline arguments.
            var commandParser = new Parser(with => with.IgnoreUnknownArguments = true);
            var parsed = commandParser.ParseArguments<CommandOptions>(args);

            var ipAddress = "";
            var tcpPort = 0;
            var message = "";
            parsed.WithNotParsed(_ =>
                {
                    var helpTest = HelpText.AutoBuild(parsed);
                    Console.WriteLine($"Parse failed: {helpTest}");
                })
                .WithParsed(o =>
                {
                    ipAddress = o.IpAddress;
                    tcpPort = o.Port;
                    message = o.Message;
                });

            // Create tcp server.
            var response = TcpClient.StartClient(ipAddress, tcpPort, message);
            Console.WriteLine(response);
        }
    }
}