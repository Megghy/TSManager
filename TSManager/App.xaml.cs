using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
                this.DispatcherUnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs ex) => Utils.Notice("UI异常捕获:\r\n" + ex);
                //非UI线程未捕获异常处理事件(例如自己创建的一个子线程)
                AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs ex) => Utils.Notice("多线程异常捕获:\r\n" + ex);
                //Task线程内未捕获异常处理事件
                TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs ex) => Utils.Notice("异步异常捕获:\r\n" + ex);

                if (TSManager.Properties.Settings.Default.UpgradeRequired)
                {
                    Utils.Notice("已更新至新版本.", HandyControl.Data.InfoType.Success);
                    TSManager.Properties.Settings.Default.Upgrade();
                    TSManager.Properties.Settings.Default.UpgradeRequired = false;
                    TSManager.Properties.Settings.Default.Save();
                }
                if (File.Exists(Info.Path + "TerrariaServer.exe"))
                {
                    Info.Server = new ServerManger(typeof(ServerApi).Assembly);
                    TSMMain.GUI.ShowDialog();
                    Environment.Exit(0);
                }
                else
                {
                    MessageBox.Show("未在目录下找到服务器启动文件.");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
    [SuppressUnmanagedCodeSecurity]
    public static class ConsoleManager
    {
        private const string Kernel32_DllName = "kernel32.dll";

        [DllImport(Kernel32_DllName)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32_DllName)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32_DllName)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport(Kernel32_DllName)]
        private static extern int GetConsoleOutputCP();

        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }

        /// <summary>  
        /// Creates a new console instance if the process is not attached to a console already.  
        /// </summary>  
        public static void Show()
        {
#if DEBUG
            if (!HasConsole)
            {
                AllocConsole();
                InvalidateOutAndError();
            }
#endif
        }

        /// <summary>  
        /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.  
        /// </summary>  
        public static void Hide()
        {
#if DEBUG
            if (HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
#endif
        }

        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        static void InvalidateOutAndError()
        {
            Type type = typeof(System.Console);

            System.Reflection.FieldInfo _out = type.GetField("_out",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.FieldInfo _error = type.GetField("_error",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.MethodInfo _InitializeStdOutError = type.GetMethod("InitializeStdOutError",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            Debug.Assert(_out != null);
            Debug.Assert(_error != null);

            Debug.Assert(_InitializeStdOutError != null);

            _out.SetValue(null, null);
            _error.SetValue(null, null);

            _InitializeStdOutError.Invoke(null, new object[] { true });
        }

        static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
        static void SetOut(string A)
        {

        }
    }
}
