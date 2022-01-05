using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using TSManager.Core.Models;

namespace TSManager.Core.Modules
{
    public static class ServerManager
    {
        public static BindingList<ServerContainer> Servers { get; internal set; } = new();
        
        public static void CreateServer(ServerInfo info)
        {

        }

        public static bool CheckIsValid(ServerInfo info)
        {
            return true; //还不知道结构
        }
        public static (bool, string) StartServer(ServerInfo targetServer)
        {
            if (Data.Servers.FirstOrDefault(s => s.Info == targetServer) is { } server)
            {
                if (server.IsRunning)
                    return (false, "指定服务器正在运行");
                else
                {
                    server.Start();
                    return (true, null);
                }
            }
            else
                return (false, "未找到指定服务器");
        }
    }
}
