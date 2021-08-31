using System;
using System.Collections.Generic;
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
    internal class ServerStatus
    {
        public ServerStatus() => Init();
        void Init()
        {
            Task.Run(() =>
            {
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
        [AlsoNotifyFor("RunTime_Text", new string[] { "CPUUsed", "MemoryUsed", "PlayerCount" })]
        public long RunTime { get; set; }
        public string RunTime_Text { get { TimeSpan ts = new(0, 0, (int)(RunTime / 1000)); return $"{ts.Days}日 {ts.Hours}时 {ts.Minutes}分 {ts.Seconds}秒"; } set { } }
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
        public int PlayerCount { get => Info.IsEnterWorld ? Info.OnlinePlayers.Count : 0; set { } }
        private readonly PerformanceCounter CPUPerformance = new("Process", "% Processor Time", "TSManager");
        private readonly PerformanceCounter MemoryPerformance = new("Process", "Working Set", "TSManager");
        public double CPUUsed
        {
            get => Math.Round(CPUPerformance.NextValue(), 2);
            set { }
        }
        public double MemoryUsed
        {
            get => Math.Round(MemoryPerformance.NextValue() / 1024 / 1024, 2);
            set { }
        }
    }
}
