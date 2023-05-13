using System.Collections.Concurrent;
using System.Diagnostics;
using System.Management;
using Netife_cli.Model;

namespace Netife_cli;

public static class CliHelper
{
    private static ConcurrentBag<Process> _processes = new ();

    public static Action<string> WriteDispatcher;
    public static void BuildServer(RuntimeConfiguration configuration)
    {
        Task.Run(() =>
        {
            Process process = new Process();
            process.StartInfo.FileName = ".\\bin\\NetifeJsRemote.exe";
            process.StartInfo.Arguments = $"{configuration.JsRemoteHost} {configuration.JsRemotePort}";
            process.StartInfo.CreateNoWindow = true; // 获取或设置指示是否在新窗口中启动该进程的值（不想弹出powershell窗口看执行过程的话，就=true）
            process.Start();
            _processes.Add(process);
            ChildProcessTracker.AddProcess(process);
        });
        
        Task.Run(() =>
        {
            Process process = new Process();
            process.StartInfo.FileName = ".\\bin\\NetifeDispatcher.exe";
            process.StartInfo.Arguments = $"{configuration.DispatcherHost} {configuration.DispatcherPort} " +
                                          $"{configuration.FrontendHost} {configuration.FrontendPort} " +
                                          $"{configuration.JsRemoteHost} {configuration.JsRemotePort}";
            process.StartInfo.CreateNoWindow = true; // 获取或设置指示是否在新窗口中启动该进程的值（不想弹出powershell窗口看执行过程的话，就=true）
            process.StartInfo.RedirectStandardOutput = true; 
            process.Start();
            _processes.Add(process);
            ChildProcessTracker.AddProcess(process);
            StreamReader reader  =  process.StandardOutput; // 截取输出流 
            string  line  =  reader.ReadLine(); // 每次读取一行 
            while (true)
            {
                if (string.IsNullOrEmpty(line))
                {
                    Thread.Sleep(100);
                }
                WriteDispatcher(line);
                line = reader.ReadLine();
                Thread.Sleep(10);
            }
        });
        
        Task.Run(() =>
        {
            Process process = new Process();
            process.StartInfo.FileName = ".\\bin\\NetifeProbe.exe";
            process.StartInfo.Arguments = $"";
            process.StartInfo.CreateNoWindow = true; // 获取或设置指示是否在新窗口中启动该进程的值（不想弹出powershell窗口看执行过程的话，就=true）
            process.Start();
            _processes.Add(process);
            ChildProcessTracker.AddProcess(process);
        });
    }

    public static void EndServices()
    {
        foreach (var process in _processes)
        {
            process.Kill();
        }
    }
    
}