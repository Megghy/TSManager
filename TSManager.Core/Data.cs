using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TSManager
{
    public static class Data
    {
        public static string CurrentPath => new DirectoryInfo(AppContext.BaseDirectory + "..\\").FullName;
        public static string LibPath => Path.Combine(CurrentPath, "Lib");
        public static string ConfigPath => Path.Combine(CurrentPath, "Config");
        public static string ScriptPath => Path.Combine(CurrentPath, "Script");
        public static string TempPath => Path.Combine(CurrentPath, "Temp");
        public static string LogPath => Path.Combine(CurrentPath, "Log");
    }
}
