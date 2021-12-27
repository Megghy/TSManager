using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using TSManager.Core.Attributes;

namespace TSManager.Core.Core.Plugin
{
    public class PluginCore
    {
        public static readonly List<TSMPluginBase> plugins = new();
        [AutoStart]
        public static void LoadAllPlugin()
        {
            if (!Directory.Exists(Data.PluginPath))
                Directory.CreateDirectory(Data.PluginPath);
            Directory.GetFiles(Data.PluginPath, "TSManager.*.dll").ForEach(path =>
            {
                //var context = new UnloadableContext(path); 本来打算做成可卸载不过感觉没啥必要, 以后用得上再加吧
                //context.Load();
                GetPluginTypes(Assembly.LoadFrom(path)).ForEach(t =>
                {
                    var plugin = (TSMPluginBase)Activator.CreateInstance(t);
                    try
                    {
                        plugin.Initialize();
                        plugins.Add(plugin);
                        Logs.Info($"- Loaded plugin: {plugin.Name} <{plugin.Author}> V{plugin.Version}");
                    }
                    catch (Exception ex)
                    {
                        Logs.Warn($"Failed to initialize plugin: {plugin.Name}{Environment.NewLine}{ex}");
                    }
                });
            });
        }
        private static IReadOnlyCollection<string> FindAssemliesWithPlugins(string path)
        {
            var assemblies = Directory.GetFiles(path, "*.dll");
            var assemblyPluginInfos = new List<string>();
            var pluginFinderAssemblyContext = new AssemblyLoadContext(name: "PluginFinderAssemblyContext", isCollectible: true);
            foreach (var assemblyPath in assemblies)
            {
                var assembly = pluginFinderAssemblyContext.LoadFromAssemblyPath(assemblyPath);
                if (GetPluginTypes(assembly).Any())
                    assemblyPluginInfos.Add(assemblyPath);
            }
            pluginFinderAssemblyContext.Unload();
            return assemblyPluginInfos;
        }
        private static Type[] GetPluginTypes(Assembly assembly)
        {
            var types = assembly.GetTypes();
            return assembly.GetTypes()
                            .Where(t => t.IsSubclassOf(typeof(TSMPluginBase)))
                            .ToArray();
        }
    }
}
