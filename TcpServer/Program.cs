using CommandLine;
using CommandLine.Text;

namespace TcpServer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // Parse commandline arguments.
            var commandParser = new Parser(with => with.IgnoreUnknownArguments = true);
            var parsed = commandParser.ParseArguments<CommandOptions>(args);

            var tcpPort = 0;
            parsed.WithNotParsed(_ =>
            {
                var helpTest = HelpText.AutoBuild(parsed);
                Console.WriteLine($"Parse failed: {helpTest}");
            })
            .WithParsed(o =>
            {
                tcpPort = o.Port;
            });

            // Create tcp server.
            await TcpServer.StartListening(tcpPort);
        }
    }
}