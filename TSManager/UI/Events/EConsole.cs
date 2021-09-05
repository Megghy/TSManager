using HandyControl.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using static TSManager.UI.GUIEvents;
using TSManager.Modules;
using System.Windows.Controls.Primitives;

namespace TSManager.UI.Events
{
    //[GUIEvent(EventType.Class, null)]
    internal class EConsole : GUIEventBase
    {
        public override string ControlPrefix => "Console";
        public override void OnButtonClick(Button sender)
        {
            switch (sender.Name)
            {
                case "Console_StartServer":
                    Info.Server.Start(Info.Server.GetStartArgs());
                    break;
                case "Console_StopServer":
                    TSMMain.GUI.Close();
                    break;
                case "Console_RestartServer":
                    if (Info.IsServerRunning)
                    {
                        Growl.Ask("服务器正在运行, 确定要关闭并重启吗?", result =>
                        {
                            if (result)
                            {
                                TSMMain.GUI.reStart = true;
                                TSMMain.Instance.StopServer();
                            }
                            return true;
                        });
                    }
                    else
                    {
                        TSMMain.GUI.Visibility = Visibility.Hidden;
                        Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
                        Environment.Exit(0);
                    }
                    break;
                case "Console_DoCmd":
                    if (!Info.IsServerRunning) return;
                    Info.Server.ExcuteConsoleCommand(TSMMain.GUI.Console_CommandBox.Text);
                    TSMMain.GUI.Console_CommandBox.Clear();
                    break;
                case "Console_Clear":
                    TSMMain.GUI.Console_ConsoleBox.Document.Blocks.Clear();
                    Utils.Notice("已清空控制台文本", HandyControl.Data.InfoType.Success);
                    break;
                default:
                    break;
            }
        }
        public override void OnSwichClick(ToggleButton sender)
        {
            switch (sender.Name)
            {
                case "Console_AutoStart":
                    TSMMain.Settings.AutoStart = (bool)sender.IsChecked;
                    break;
                case "Console_EnableChinese":
                    TSMMain.Settings.EnableChinese = (bool)sender.IsChecked;
                    break;
                default:
                    break;
            }
            TSMMain.Settings.Save();
        }
    }
}
