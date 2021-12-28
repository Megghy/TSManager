using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.Core.Models
{
    public class UnloadableContext
    {
        public UnloadableContext()
        {
            Context = new AssemblyLoadContext($"TSManagerContext_{Guid.NewGuid()}", isCollectible: true);
            Context.Resolving += ContextResolving;
        }

        private Assembly? ContextResolving(AssemblyLoadContext context, AssemblyName name)
        {
            string assemblyPath = $"{FileDirectory}\\{name.Name}.dll";
            if (assemblyPath != null)
                return context.LoadFromAssemblyPath(assemblyPath);
            return null;
        }

        public UnloadableContext(string path) : this()
        {
            FilePath = path;
        }
        public string FilePath { get; set; }
        public string FileDirectory => Path.GetDirectoryName(FilePath);
        public bool IsLoaded { get; private set; }
        public AssemblyLoadContext Context { get; private set; }
        public Assembly MainAssembly { get; private set; }
        /// <summary>
        /// 从指定路径中加载
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            MainAssembly = Context.LoadFromAssemblyPath(FilePath);
            IsLoaded = true;
            return true; //临时测试
            if (File.Exists(FilePath) && !IsLoaded)
            {
                MainAssembly = Context.LoadFromAssemblyPath(FilePath);
                IsLoaded = true;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 卸载加载的程序集
        /// </summary>
        public void Unload()
        {
            Context.Unload();
            IsLoaded = false;
            MainAssembly = null;
            Context = null;
        }
    }
}
