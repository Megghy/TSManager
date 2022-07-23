using StreamJsonRpc;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using TSManager.Core.Events;
using TSManager.Core.Modules;
using TSManager.Shared.TSMDatastructs;
using TSManager.Shared.TSMInterfaces;

namespace TSManager.Core.Models
{
    public class ServerContainer : IDisposable
    {
        public ServerContainer(ServerInfo info, bool createService = true)
        {
            _createService = createService;
            Info = info;
            Init(createService);
        }

        #region 成员
        private bool _createService;

        public ServerInfo Info { get; init; }
        public string FilePath => Info.FilePath;
        public string[] StartArgs { get; set; } = Array.Empty<string>();
        private SimplePipeline _bindPipeline;
        private IRPCServerService _rpcProcessor;
        private JsonRpc _rpcInstance;
        public IRPCClientService RPCService { get; private set; }

        public BindingList<PluginInfo> Plugins { get; set; }
        public Process ServerProcess { get; private set; }

        public bool IsRunning { get; private set; }
        public bool IsTrServer { get; internal set; }

        public long RunTime { get; private set; }     
        #endregion

        #region 启动关闭
        private void Init(bool createService)
        {
            ServerProcess = new()
            {
                StartInfo = new()
                {
                    FileName = FilePath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                }
            };
            ServerProcess.OutputDataReceived += OnRecieveOutput;

            if (createService)
            {
                _bindPipeline = new();
                _rpcProcessor = new DefaultRPCServerProcessor(this);
            }
        }
        public async Task<bool> StartAsync(string[] args = null)
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
                if (_createService)
                {
                    await _bindPipeline.StartAsync();
                    _rpcInstance = JsonRpc.Attach(_bindPipeline._pipeServer, _rpcProcessor);
                    RPCService = _rpcInstance.Attach<IRPCClientService>();

                    Logger.Success($"实例 [{Info.Name}] RPC服务已注册");
                }
                Logger.Success($"实例 [{Info.Name}] 启动完成");
                return true;
            }
        }

        public async Task StopAsync()
        {
            if (IsRunning)
                Logger.Info($"实例 [{Info.Name}] 未启动, 无法停止");
            else
            {
                IsRunning = false;
                if (IsTrServer) //tr服务器退出使用exit
                {
                    WriteLine("exit");
                    await ServerProcess.WaitForExitAsync();
                }
                else
                {
                    ServerProcess.Kill();
                }
                ServerProcess = null;
                _bindPipeline.Dispose();
                _bindPipeline = null;
                Logger.Success($"实例 [{Info.Name}] 已停止");
            }
        }
        public async Task RestartAsync()
        {
            await StopAsync();
            Init(_createService);
            await StartAsync();
        }
        public async Task DisposeAsync()
        {
            await StopAsync();
            Plugins = null;
        }
        public void Dispose()
            => DisposeAsync().Wait();
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
                ServerEvent.OnOutput(this, new ConsoleMessageInfo[] { new() { Text = e.Data } }, out _);
        }
        #endregion

        #region 操作trserver进程
        /// <summary>
        /// 向进程输入流写入消息
        /// </summary>
        /// <param name="obj">消息</param>
        public void WriteLine(object obj)
        {
            ServerProcess?.StandardInput.WriteLine(obj?.ToString());
        }

        #endregion
    }
}
