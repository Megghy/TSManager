using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TSManager.Core.Attributes;

namespace TSManager.Core
{
    public class Program
    {
        private static bool _initialized = false;
        public static void Main(string[] args)
        {
            if (_initialized)
                return;
            _initialized = true;
            
            SetupFolder();

            Logs.Info("初始化 TSManager 核心...");

            InvokeAllAutoStartMethods();
        }

        public static void InvokeAllAutoStartMethods()
        {
            Logs.Info("加载所有需加载项");
            var inits = new List<MethodInfo>();
            Assembly.GetExecutingAssembly()
                .GetTypes()
                .ForEach(t => t.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.GetCustomAttribute<AutoStartAttribute>() is { }).ForEach(m => inits.Add(m)));
            inits = inits.OrderBy(m => m.GetCustomAttribute<AutoStartAttribute>().Order).ToList();
            inits.ForEach(m =>
            {
                Logs.Info($"<{m.DeclaringType.Name}.{m.Name}> => Init.");
                var attr = m.GetCustomAttribute<AutoStartAttribute>();
                if(!string.IsNullOrEmpty(attr.PreMessage))
                    Logs.Info(attr.PreMessage);
                m.Invoke(null, null);
                if (!string.IsNullOrEmpty(attr.PostMessage))
                    Logs.Info(attr.PreMessage);
            });
            Logs.Success("加载完成");
        }

        private static void SetupFolder()
        {
            if (!Directory.Exists(Data.LibPath))
                Directory.CreateDirectory(Data.LibPath);
            if (!Directory.Exists(Data.LogPath))
                Directory.CreateDirectory(Data.LogPath);
            if (!Directory.Exists(Data.ConfigPath))
                Directory.CreateDirectory(Data.ConfigPath);
            if (!Directory.Exists(Data.ScriptPath))
                Directory.CreateDirectory(Data.ScriptPath);
            if (!Directory.Exists(Data.TempPath))
                Directory.CreateDirectory(Data.TempPath);
        }
    }
}
