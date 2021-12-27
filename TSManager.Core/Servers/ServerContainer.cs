using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TSManager.Core.Models;

namespace TSManager.Core.Servers
{
    public class ServerContainer : UnloadableContext
    {
        public ServerContainer() : base() { }
        public ServerContainer(string name, string serverPath, string[] args) : base(serverPath)
        {
            Name = name;
            StartArgs = args;
        }

        #region 成员
        public string Name { get; private set; } = string.Empty;
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
