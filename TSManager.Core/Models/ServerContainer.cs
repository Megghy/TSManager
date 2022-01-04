using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TSManager.Core.Models
{
    public class ServerContainer
    {
        public ServerContainer(ServerInfo info, string[] args = null)
        {
            Info = info;
            StartArgs = args ?? Array.Empty<string>();

            ServerProcess = new()
            {
                StartInfo = new()
                {
                    FileName = FilePath,
                    Arguments = string.Join(" ", args),
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

        public bool IsTrServer { get; internal set; }
        #endregion

        #region 启动关闭
        public void Start()
        {
            ServerProcess.Start();
        }

        public void Stop()
        {
            ServerProcess.Kill();
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
