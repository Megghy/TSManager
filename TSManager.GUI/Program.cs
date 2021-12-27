using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TSManager.GUI
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                args = args.Select(a => a.ToLower()).ToArray();

                Core.Program.Main(args);

                if (!args.Contains("noweb")
                    && Path.Combine(Data.LibPath!, "TSManager.Web.dll") is { } path
                    && File.Exists(path))
                {
                    Assembly.LoadFrom(path);
                    Task.Run(() => typeof(Web.App).Assembly.EntryPoint?.Invoke(null, new object[] { args }));
                }

                Logs.Info("启动用户界面");
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
            catch (Exception ex)
            {
                Logs.Error($"初始化失败{Environment.NewLine}{ex}");
            }
        }
    }
}
