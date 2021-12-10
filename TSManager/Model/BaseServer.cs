using System;
using System.IO;
using System.Runtime.Loader;
using System.Reflection;

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
}
