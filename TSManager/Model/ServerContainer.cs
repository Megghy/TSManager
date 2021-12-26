using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.Model
{
    public abstract class BaseServer
    {
        protected AssemblyLoadContext serverContext;
        public Assembly assembly;
        public BaseServer()
        {
            serverContext = new AssemblyLoadContext($"TSManagerServerContainer_{Guid.NewGuid()}", isCollectible: true);
        }
        public BaseServer(string serverPath, string[] args) : this()
        {
            serverFilePath = serverPath;
            startArgs = args;
        }
        public string name;
        public string serverFilePath;
        public string[] startArgs = Array.Empty<string>();
        /// <summary>
        /// 从指定路径中加载
        /// </summary>
        /// <returns></returns>
        public virtual bool Load()
        {
            if (File.Exists(serverFilePath))
            {
                assembly = serverContext.LoadFromAssemblyPath(serverFilePath);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 卸载加载的程序集
        /// </summary>
        public virtual void Unload()
        {
            serverContext.Unload();
            serverContext = null;
        }
    }
    public class ServerContainer : BaseServer
    {
        public ServerContainer() : base() { }
        public ServerContainer(string serverPath, string[] args) : base(serverPath, args) { }

        #region 成员
        public BindingList<TerrariaApi.Server.TerrariaPlugin> Plugins { get; set; }
        public Task serverTask { get; private set; }
        #endregion
        #region 启动关闭
        public void Start(string[] args = null)
        {
            Load();
            serverTask = new Task(() => assembly?.EntryPoint.Invoke(null, new object[] { args ?? startArgs }));
            serverTask.Start();
            RedirectOutput();
        }
        
        public void Stop()
        {
            serverTask.Dispose();
            Unload();
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
