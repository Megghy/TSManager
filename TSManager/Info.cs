using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;

namespace TSManager
{
    class Info
    {
        public static Modules.ServerManger Server;
        public static string Path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        public static string ConfigPath => Path + "tshock\\";
        public static string PluginPath => Path + "ServerPlugins\\";
        /// <summary>
        /// 服务器线程状态
        /// </summary>
        public static bool IsServerRunning = false;
        /// <summary>
        /// 是否已经选择世界并进入
        /// </summary>
        public static bool IsEnterWorld = false;
        /// <summary>
        /// 运行tr服务器的线程
        /// </summary>
        public static Thread GameThread;

        public static Ionic.Zip.ZipFile TextureZip;
        public static List<Data.ConfigData> Configs;
        public static List<Data.PlayerInfo> Players;
        
    }
}
