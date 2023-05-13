using Netife_cli.Services;
using NetifeMessage;
using Terminal.Gui;

namespace Netife_cli;

public static class Commander
{
    public static Action<string> Write;
    
    public static Action<string> WriteLine;
    


    public static NetifeService.NetifeServiceClient Client;
    public static void Command(string cmd)
    {
        var header = cmd.Split(" ")[0];
        string[] paras;
        switch (header)
        {
            case "quit":
                End();
                Application.Shutdown();
                Environment.Exit(0);
                break;
            case "mode":
                if (TryInjectParams(cmd, 1, out paras))
                {
                    ModeChange(paras);
                }
                break;
            case "command":
                var commandRes = RequestRemoteCommand(cmd.Substring(8, cmd.Length - 8));
                if (string.IsNullOrEmpty(commandRes))
                {
                    WriteLine("Error in command params or undefined command");
                    break;
                }
                WriteLine("[PluginBack]" + commandRes);
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
                    Write($"Catch Connection to {sp.DstIpAddr}:80, from {sp.Pid} with content\n {sp.RawText}\n");
                    return res;
                };
                WriteLine("Change to Listen Mode");
                break;
            case "silent":
                FrontendService.action = sp =>
                {
                    var res = new NetifeProbeResponse();
                    res.Uuid = sp.Uuid;
                    res.DstIpAddr = sp.DstIpAddr;
                    res.DstIpPort = sp.DstIpPort;
                    res.ResponseText = sp.RawText;
                    return res;
                };
                WriteLine("Change to Silent Mode");
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
    
    private static string RequestRemoteCommand(string rawCommand)
    {
        var request = new NetifePluginCommandRequest();
        
        var paras = SplitWithoutBlank(rawCommand);

        request.CommandPrefix = paras[0];
        request.Params.AddRange(paras);
        NetifePluginCommandResponse? res;
        try
        {
            res = Client.Command(request);
        }
        catch (Exception e)
        {
            return string.Empty;
        }
        if (res.Status)
        {
            return res.Result;
        }
        return string.Empty;
    }

    private static List<string> SplitWithoutBlank(string rawText)
    {
        var split = rawText.Trim().Split(' ');

        var result = new List<string>();
        var current = "";

        foreach (var segment in split)
        {
            current += segment;
            if (current.Count(c => c == '"') % 2 == 0)
            {
                result.Add(current.Trim().Trim('"'));
                current = "";
            }
            else
            {
                current += " ";
            }
        }

        if (!string.IsNullOrEmpty(current))
        {
            result.Add(current.Trim().Trim('"'));
        }

        return result.ToList();
    }
}