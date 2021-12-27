// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.IO;
using System.Reflection;

if (Environment.OSVersion.Platform == PlatformID.Win32NT)
{
    var path = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "Lib"), "TSManager.GUI.dll");
    if (File.Exists(path))
    {
        Console.WriteLine($"[Info] 运行于Windows, 启动交互界面及Web库");
        Process.Start(path);
    }
    else 
        Console.WriteLine($"[Warn] 未找到 TSManager 界面库");
}
else
{
    var path = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "Lib"), "TSManager.Web.dll");
    if (File.Exists(path))
    {
        Console.WriteLine($"[Info] 运行于非Windows环境, 仅启用Web库"); 
        Process.Start(path);
    }
    else
        Console.WriteLine($"[Warn] 未找到 TSManager Web库");
}
Thread.Sleep(2000);

