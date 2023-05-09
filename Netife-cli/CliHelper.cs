using System.Collections.Concurrent;
using System.Diagnostics;
using Netife_cli.Model;

namespace Netife_cli;

public static class CliHelper
{
    private static ConcurrentBag<Process> _processes = new ();
    public static void BuildServer(RuntimeConfiguration configuration)
    {
        Task.Run(() =>
        {
            Process process = new Process();
            process.StartInfo.FileName = ".\\bin\\NetifeJsRemote.exe";
            process.StartInfo.Arguments = $"{configuration.JsRemoteHost} {configuration.JsRemotePort}";
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.CreateNoWindow = true; // 获取或设置指示是否在新窗口中启动该进程的值（不想弹出powershell窗口看执行过程的话，就=true）
            process.StartInfo.ErrorDialog = true; // 该值指示不能启动进程时是否向用户显示错误对话框
            process.StartInfo.UseShellExecute = false;
            process.Start();
            _processes.Add(process);
        });
        
        Task.Run(() =>
        {
            Process process = new Process();
            process.StartInfo.FileName = ".\\bin\\NetifeDispatcher.exe";
            process.StartInfo.Arguments = $"{configuration.DispatcherHost} {configuration.DispatcherPort} " +
                                          $"{configuration.FrontendHost} {configuration.FrontendPort} " +
                                          $"{configuration.JsRemoteHost} {configuration.JsRemotePort}";
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.CreateNoWindow = true; // 获取或设置指示是否在新窗口中启动该进程的值（不想弹出powershell窗口看执行过程的话，就=true）
            process.StartInfo.ErrorDialog = true; // 该值指示不能启动进程时是否向用户显示错误对话框
            process.StartInfo.UseShellExecute = false;
            process.Start();
            _processes.Add(process);
        });
        
        Task.Run(() =>
        {
            Process process = new Process();
            process.StartInfo.FileName = ".\\bin\\NetifeProbe.exe";
            process.StartInfo.Arguments = $"";
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.CreateNoWindow = true; // 获取或设置指示是否在新窗口中启动该进程的值（不想弹出powershell窗口看执行过程的话，就=true）
            process.StartInfo.ErrorDialog = true; // 该值指示不能启动进程时是否向用户显示错误对话框
            process.StartInfo.UseShellExecute = false;
            process.Start();
            _processes.Add(process);
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