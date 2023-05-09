// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Netife_cli;
using Netife_cli.Model;
using Netife_cli.Services;

Console.WriteLine("Welcome to use Netife-cli.");
Console.WriteLine("Netife Cli: v1.0.0 Beta");
Console.WriteLine("Netife Cli will ensure its relative components updated.");
Console.WriteLine("Trying to build running server according to config...");

var configuration = new RuntimeConfiguration();
CliHelper.BuildServer(configuration);

new Server
{
    Services =
    {
        NetifeMessage.NetifePost.BindService(new FrontendService())
    },
    Ports = { new ServerPort(configuration.FrontendHost, int.Parse(configuration.FrontendPort), ServerCredentials.Insecure) }
}.Start();

Console.WriteLine("[Netife]Auto entering stream mode.");

//关闭事件
Console.CancelKeyPress += (sender, eventArgs) =>
{
    CliHelper.EndServices();
};

while (true)
{
    Console.Write("Netife >>> ");
    string cmd = Console.ReadLine() ?? String.Empty;
    Commander.Command(cmd);
}