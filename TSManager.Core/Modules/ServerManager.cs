using System;
using System.ComponentModel;
using System.Linq;
using TSManager.Core.Models;

namespace TSManager.Core.Modules
{
    public static class ServerManager
    {
        public static BindingList<ServerContainer> Servers { get; internal set; } = new();

        public static (bool, ServerContainer) CreateServer(ServerInfo info)
        {
            try
            {
                if (CheckIsValid(info))
                    return (true, new ServerContainer(info));
                else
                    return (false, null);
            }
            catch (Exception ex)
            {
                Logger.Error($"未能创建服务器实例{Environment.NewLine}{ex}");
                return (false, null);
            }
        }

        public static bool CheckIsValid(ServerInfo info)
        {
            return true; //还不知道结构
        }
        public static async Task<(bool, string)> StartServerAsync(ServerInfo targetServer)
        {
            if (Data.Servers.FirstOrDefault(s => s.Info == targetServer) is { } server)
            {
                if (server.IsRunning)
                    return (false, "指定服务器正在运行");
                else
                {
                    await server.StartAsync();
                    return (true, null);
                }
            }
            else
                return (false, "未找到指定服务器");
        }
    }
}
