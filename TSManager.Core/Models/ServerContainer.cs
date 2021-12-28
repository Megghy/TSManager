using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TSManager.Core.Models
{
    public class ServerContainer : UnloadableContext
    {
        public ServerContainer(ServerInfo info, string[] args = null) : base(info.ServerFilePath)
        {
            Info = info;
            StartArgs = args ?? Array.Empty<string>();
        }

        #region 成员
        public ServerInfo Info { get; set; }
        public string[] StartArgs { get; set; } = Array.Empty<string>();
        public BindingList<TerrariaApi.Server.TerrariaPlugin> Plugins { get; set; }
        public Task ServerTask { get; private set; }
        #endregion

        #region 启动关闭
        public void Start(string[] args = null)
        {
            Load();
            ServerTask = new Task(() => MainAssembly?.EntryPoint.Invoke(null, new object[] { args ?? StartArgs }));
            ServerTask.Start();
            RedirectOutput();
        }

        public void Stop()
        {
            ServerTask.Dispose();
            Unload();
        }
        #endregion

        #region 操作trserver进程
        private void RedirectOutput()
        {
            Context.Assemblies.SingleOrDefault(a => a.FullName.Contains("console"));
        }
        #endregion
    }
}
