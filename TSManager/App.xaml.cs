using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TerrariaApi.Server;
using TSManager.Modules;

namespace TSManager
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static App Instance;
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                Instance = this;
                //UI线程未捕获异常处理事件（UI主线程）
                DispatcherUnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs ex) => Utils.Notice("UI异常捕获:\r\n" + ex.Exception.Message);
                //非UI线程未捕获异常处理事件(例如自己创建的一个子线程)
                AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs ex) =>
                {
                    Utils.Notice($"发生无法处理的异常, 程序即将退出\r\n请向开发者报告此问题\r\n{ex}\r\n{ex.ExceptionObject.GetType().FullName}");
                };
                //Task线程内未捕获异常处理事件
                TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs ex) => Utils.Notice("异步异常捕获:\r\n" + ex.Exception.Message);
                Exit += (_, _) => TSManager.Properties.Settings.Default.Save();

                if (TSManager.Properties.Settings.Default.UpgradeRequired)
                {
                    Utils.Notice("已更新至新版本.", HandyControl.Data.InfoType.Success);
                    TSManager.Properties.Settings.Default.Upgrade();
                    TSManager.Properties.Settings.Default.UpgradeRequired = false;
                    TSManager.Properties.Settings.Default.Save();
                }
                if (!File.Exists(Info.Path + "TerrariaServer.exe"))
                {
                    MessageBox.Show("未在目录下找到服务器启动文件.");
                    return;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
