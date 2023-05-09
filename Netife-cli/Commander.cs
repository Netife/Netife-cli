using Netife_cli.Services;
using NetifeMessage;

namespace Netife_cli;

public static class Commander
{
    public static void Command(string cmd)
    {
        var header = cmd.Split(" ")[0];
        string[] paras;
        switch (header)
        {
            case "quit":
                End();
                break;
            case "mode":
                if (TryInjectParams(cmd, 1, out paras))
                {
                    ModeChange(paras);
                }
                break;
            default:
                Console.WriteLine("Missing command match, please check your command.");
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
                    Console.Write($"\n[Netife] Catch Connection to {sp.DstIpAddr}:80, from {sp.Pid}\nNetife >>>");
                    return res;
                };
                Console.WriteLine("[Netife]Change to Listen Mode");
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