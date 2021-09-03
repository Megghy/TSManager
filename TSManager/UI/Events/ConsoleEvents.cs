using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static TSManager.UI.GUIEvent;
using static TSManager.TSMMain;

namespace TSManager.UI.Events
{
    [GUIEvent]
    internal class ButtonClickEvent_Console : GUIEventBase
    {
        public override string ControlPrefix => "Console";
        public override EventType TargetEvent => EventType.ButtonClick;
        public override void OnEvent(object sender, string name)
        {
            switch (InternalName(name))
            {
                case "StartServer":
                    Info.Server.Start(Info.Server.GetStartArgs());
                    break;
                case "StopServer":
                    GUI.Close();
                    break;
                case "RestartServer":
                    if (Info.IsServerRunning)
                    {
                        Growl.Ask("服务器正在运行, 确定要关闭并重启吗?", result =>
                        {
                            if (result)
                            {
                                GUI.reStart = true;
                                Instance.StopServer();
                            }
                            return true;
                        });
                    }
                    else
                    {
                        GUI.Visibility = Visibility.Hidden;
                        Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
                        Environment.Exit(0);
                    }
                    break;
                case "DoCmd":
                    if (!Info.IsServerRunning) return;
                    Info.Server.ExcuteConsoleCommand(GUI.Console_CommandBox.Text);
                    GUI.Console_CommandBox.Clear();
                    break;
                case "Clear":
                    GUI.Console_ConsoleBox.Document.Blocks.Clear();
                    Utils.Notice("已清空控制台文本", HandyControl.Data.InfoType.Success);
                    break;
            }
        }
    }
}
