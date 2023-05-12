// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Grpc.Net.Client;
using Netife_cli;
using Netife_cli.Model;
using Netife_cli.Services;
using NetifeMessage;
using Terminal.Gui;



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

//关闭事件
Console.CancelKeyPress += (sender, eventArgs) =>
{
    CliHelper.EndServices();
};

Application.Init();

var text = new TextView() {
    X = 0,
    Y = 0,
    Width = Dim.Fill(),
    Height = Dim.Fill() - 2,
    ColorScheme = new ColorScheme {
        Normal = Terminal.Gui.Attribute.Make(Color.Black, Color.White),
    },
    Multiline = true,
    ReadOnly = true,
    WordWrap = true
};

var input = new TextField()
{
    X = 11,
    Y = Pos.Bottom(text) + 1,
    Height = 1,
    Width = Dim.Fill() - 11,
    ColorScheme = new ColorScheme {
        Normal = Terminal.Gui.Attribute.Make(Color.Black, Color.White),
    }
};

var label = new Label("Netife >>>")
{
    X = 0,
    Y = Pos.Bottom(text) + 1,
    Height = 1,
    ColorScheme = new ColorScheme {
        Normal = Terminal.Gui.Attribute.Make(Color.Black, Color.White),
    }
};

text.Text = "Welcome to use Netife-cli.\nNetife Cli: v1.0.0 Beta\nNetife Cli will ensure its relative components updated.\n" +
                "Trying to build running server according to config...\nYou can enter command in the bottom of the Console.\n";

Commander.Write = sp => { text.Text += $"[{DateTime.Now:HH:mm:ss}]" + sp; text.MoveEnd(); Application.Refresh();};
Commander.WriteLine = sp => { text.Text += $"[{DateTime.Now:HH:mm:ss}]" + sp + "\n"; text.MoveEnd(); Application.Refresh(); };
Commander.Client =
    new NetifeService.NetifeServiceClient(
        GrpcChannel.ForAddress(configuration.DispatcherHost.Replace("0.0.0.0", "http://localhost") + ":" + configuration.DispatcherPort));

input.KeyPress += sp =>
{
    if (sp.KeyEvent.Key == Key.Enter)
    {
        Commander.Command(input.Text.ToString()!);
        input.Text = "";
        sp.Handled = true;
    }
};

Application.Top.Add(text, input, label);
Application.Run();
input.SetFocus();
CliHelper.EndServices();
Application.Shutdown();