using Netife_cli.Services;
using NetifeMessage;
using Terminal.Gui;

namespace Netife_cli;

public static class Commander
{
    public static Action<string> Write;
    
    public static Action<string> WriteLine;
    public static void Command(string cmd)
    {
        var header = cmd.Split(" ")[0];
        string[] paras;
        switch (header)
        {
            case "quit":
                End();
                Application.Shutdown();
                break;
            case "mode":
                if (TryInjectParams(cmd, 1, out paras))
                {
                    ModeChange(paras);
                }
                break;
            default:
                WriteLine("Missing command match, please check your command.");
                break;
        }
    }

    private static void End()
    {
        CliHelper.EndServices();
    }

    private static void ModeChange(string[] paras)
    {
        switch (paras[0])
        {
            case "listen":
                FrontendService.action = sp =>
                {
                    var res = new NetifeProbeResponse();
                    res.Uuid = sp.Uuid;
                    res.DstIpAddr = sp.DstIpAddr;
                    res.DstIpPort = sp.DstIpPort;
                    res.ResponseText = sp.RawText;
                    Write($"Catch Connection to {sp.DstIpAddr}:80, from {sp.Pid} with content {sp.RawText}\n");
                    return res;
                };
                WriteLine("Change to Listen Mode");
                break;
        }
    }

    private static bool TryInjectParams(string cmd, int count, out string[] paras)
    {
        var line = cmd.Split(" ");
        var num = line.Count() - 1;
        paras = line.Skip(1).ToArray();
        return num >= count;
    }
}