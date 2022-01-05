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
    public static class Init
    {
        private static bool _initialized = false;
        public static void Start(string[] args)
        {
            if (_initialized)
                return;
            _initialized = true;
            
            SetupFolder();

            Logger.Text("初始化 TSManager 核心...");

            InvokeAllAutoStartMethods();
        }

        public static void InvokeAllAutoStartMethods()
        {
            Logger.Text("加载所有需加载项");
            var inits = new List<MethodInfo>();
            Assembly.GetExecutingAssembly()
                .GetTypes()
                .ForEach(t => t.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.GetCustomAttribute<AutoStartAttribute>() is { }).ForEach(m => inits.Add(m)));
            inits = inits.OrderBy(m => m.GetCustomAttribute<AutoStartAttribute>().Order).ToList();
            inits.ForEach(async m =>
            {
                Logger.Text($"<{m.DeclaringType.Name}.{m.Name}> => Init.");
                var attr = m.GetCustomAttribute<AutoStartAttribute>();

                if(!string.IsNullOrEmpty(attr.PreMessage))
                    Logger.Text(attr.PreMessage);

                if (m.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null)
                {
                    if (m.ReturnType.IsGenericType)
                    {
                        await(dynamic)(m.Invoke(null, null) ?? Task.Delay(1));
                    }
                    else
                    {
                        await(Task)(m.Invoke(null, null) ?? Task.Delay(1));
                    }
                }
                else
                    m.Invoke(null, null);

                if (!string.IsNullOrEmpty(attr.PostMessage))
                    Logger.Text(attr.PreMessage);
            });
            Logger.Success("加载完成");
        }

        private static void SetupFolder()
        {
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
