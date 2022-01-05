using System;
using System.ComponentModel;
using System.Diagnostics;

namespace TSManager.Core.Models
{
    public class ServerContainer
    {
        public ServerContainer(ServerInfo info)
        {
            Info = info;

            ServerProcess = new()
            {
                StartInfo = new()
                {
                    FileName = FilePath,                    
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
        }

        #region 成员
        public ServerInfo Info { get; init; }
        public string FilePath => Info.FilePath;
        public string[] StartArgs { get; set; } = Array.Empty<string>();
        public BindingList<PluginInfo> Plugins { get; set; }
        public Process ServerProcess { get; private set; }

        public bool IsRunning { get; private set; }
        public bool IsTrServer { get; internal set; }

        public long RunningTime { get; private set; }
        #endregion

        #region 启动关闭
        public void Start(string[] args = null)
        {
            if (args is not null)
                ServerProcess.StartInfo.Arguments = string.Join(" ", args);
            IsRunning = true;
            ServerProcess.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            ServerProcess.Kill();
        }

        public void Dispose()
        {
            Stop();
        }
        #endregion

        #region 事件

        #endregion

        #region 操作trserver进程
        private void RedirectOutput()
        {

        }
        #endregion
    }
}
