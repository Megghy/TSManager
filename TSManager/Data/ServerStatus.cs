using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using PropertyChanged;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace TSManager.Data
{
    [AddINotifyPropertyChangedInterface]
    class ServerStatus
    {
        public ServerStatus() => LoadInfo();
        async void LoadInfo()
        {
            await Task.Run(() => {
                TSManagerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                TerrariaVersion = Main.versionNumber2.ToString();
                TShockVersion = TShock.VersionNum.ToString();
                RunTime = 0;
                var list = new List<PluginInfo>();
                foreach (var p in ServerApi.Plugins)
                {
                    list.Add(new PluginInfo(p.Plugin.Name, p.Plugin.Version.ToString(), p.Plugin.Author, p.Plugin.Description));
                }
                PluginList = list;
            });
        }
        public string TSManagerVersion { get; set; }
        public string TerrariaVersion { get; set; }
        public string TShockVersion { get; set; }
        [AlsoNotifyFor("CPU", new string[]{ "RunTime_Text" , "Memory" })]
        public long RunTime { get; set; }
        public string RunTime_Text { get { TimeSpan ts = new TimeSpan(0, 0, (int)(RunTime / 1000)); return $"{ts.Days}日 {ts.Hours}时 {ts.Minutes}分 {ts.Seconds}秒"; } set { } }
        public struct PluginInfo
        {
            public PluginInfo(string n, string v, string a, string d)
            {
                Name = n;
                Version = v;
                Author = a;
                Description = d;
            }
            public string Name { get; set; }
            public string Version { get; set; }
            public string Author { get; set; }
            public string Description { get; set; }
        }
        public List<PluginInfo> PluginList { get; set; }
        public double CPU
        {
            get
            {
                return new PerformanceCounter("Processor", "% Processor Time", "_Total").NextValue();
            }
            set { } 
        }
        public double Memory
        {
            get
            {
                return new PerformanceCounter("Memory", "Available MBytes").NextValue();
            }
            set { }
        }
    }
}
