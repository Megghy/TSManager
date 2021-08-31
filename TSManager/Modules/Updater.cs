using AutoUpdaterDotNET;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TSManager.Modules
{
    internal class Updater
    {
        private static bool firstCheckUpdate = true;
        public static readonly string UpdateURL = "https://oss.suki.club/TSManager/Update.xml";
        internal static void Init()
        {
            AutoUpdater.Start(UpdateURL);
            Timer t = new()
            {
                Interval = 60000,
                AutoReset = true
            };
            t.Elapsed += (object o, ElapsedEventArgs args) => AutoUpdater.Start("https://oss.suki.club/TSManager/Update.xml");
            t.Start();
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdater.ApplicationExitEvent += () =>
            {
                if (Info.IsEnterWorld)
                    Info.Server.ExcuteConsoleCommand("save");
                else
                    Environment.Exit(0);
            };
        }
        private static void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.IsUpdateAvailable)
            {
                if (args.Mandatory.Value)
                {
                    try
                    {
                        TSMMain.GUIInvoke(() =>
                        {
                            TSMMain.GUI.TSMVersion_Badge.ShowBadge = true;
                            TSMMain.GUI.TSMVersion_Badge.Text = args.CurrentVersion;
                            TSMMain.GUI.TSMVersion.Text = "当前版本: 发现新版本";
                        });
                        if (firstCheckUpdate)
                        {
                            if (Info.IsEnterWorld) Growl.Ask("发现新版本, 是否现在更新?", result =>
                            {
                                if (result) 
                                    AutoUpdater.ShowUpdateForm(args);
                                return true;
                            });
                            else
                                AutoUpdater.ShowUpdateForm(args);
                        }
                        firstCheckUpdate = false;
                    }
                    catch (Exception exception)
                    {
                        Utils.Notice("无法下载更新\r\n" + exception.Message, HandyControl.Data.InfoType.Error);
                    }
                }
            }
        }
    }
}
