using System;
using System.ComponentModel;
using System.Diagnostics;
using TSManager.Core.Events;
using TSManager.Core.Modules;

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
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                }
            };
            ServerProcess.OutputDataReceived += OnRecieveOutput;
            BindingPipe = new Pipeline();
        }

        #region 成员
        public ServerInfo Info { get; init; }
        public string FilePath => Info.FilePath;
        public string[] StartArgs { get; set; } = Array.Empty<string>();
        public Pipeline BindingPipe { get; private set; }
        public BindingList<PluginInfo> Plugins { get; set; }
        public Process ServerProcess { get; private set; }

        public bool IsRunning { get; private set; }
        public bool IsTrServer { get; internal set; }

        public long RunningTime { get; private set; }     
        #endregion

        #region 启动关闭
        public bool Start(string[] args = null)
        {
            if(IsRunning)
            {
                Logger.Warn($"实例 [{Info.Name}] 已在运行中");
                return false;
            }
            else
            {
                Logger.Info($"实例 [{Info.Name}] 启动中");
                if (args is not null)
                    ServerProcess.StartInfo.Arguments = string.Join(" ", args);
                IsRunning = true;
                ServerProcess.Start();
                BindingPipe.Start();
                Logger.Success($"实例 [{Info.Name}] 启动完成");
                return true;
            }
        }

        public void Stop()
        {
            if (IsRunning)
                Logger.Info($"实例 [{Info.Name}] 未启动, 无法停止");
            else
            {
                IsRunning = false;
                ServerProcess.Kill();
                BindingPipe.Dispose();
                Logger.Success($"实例 [{Info.Name}] 已停止");
            }
        }
        public void Restart()
        {
            Stop();
            BindingPipe = new();
            Start();
        }
        public void Dispose()
        {
            Stop();
            BindingPipe = null;
            ServerProcess = null;
            Plugins = null;
        }
        #endregion

        #region 事件
        /// <summary>
        /// 接收到进程输出流的消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRecieveOutput(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
                ServerEvent.OnOutput(this, new TextInfo[] { new() { Text = e.Data } }, out _);
        }
        #endregion

        #region 操作trserver进程
        /// <summary>
        /// 向进程输入流写入消息
        /// </summary>
        /// <param name="obj">消息</param>
        public void WriteLine(object obj)
        {
            ServerProcess?.StandardInput.WriteLine(obj.ToString());
        }
        #endregion
    }
}
