using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.Model
{
    public class ServerContainer : BaseServer
    {
        public ServerContainer() : base() { }
        public ServerContainer(string serverPath, string[] args) : base(serverPath, args) { }

        #region 成员
        public BindingList<TerrariaApi.Server.TerrariaPlugin> Plugins { get; set; }
        #endregion
        #region 启动关闭
        public void Start(string[] args = null)
        {
            Load();
            assembly?.EntryPoint.Invoke(null, new object[] { args ?? startArgs });

            RedirectOutput();
        }
        public void Stop()
        {

        }
        #endregion

        #region 操作trserver进程
        private void RedirectOutput()
        {
            serverContext.Assemblies.SingleOrDefault(a => a.FullName.Contains("console"));
        }
        #endregion
    }
}
