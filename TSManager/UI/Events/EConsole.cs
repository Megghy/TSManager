using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using HandyControl.Controls;
using TSManager.Data;
using TSManager.Modules;
using static TSManager.UI.GUIEvents;
using ComboBox = HandyControl.Controls.ComboBox;

namespace TSManager.UI.Events
{
    internal class EConsole : GUIEventBase
    {
        public override string ControlPrefix => "Console";
        public override void OnButtonClick(Button sender, RoutedEventArgs e)
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
                    Info.Server.AppendText(TSMMain.GUI.Console_CommandBox.Text);
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
        public override void OnSwichClick(ToggleButton sender, RoutedEventArgs e)
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
        public override void OnComboSelectChange(ComboBox sender, SelectionChangedEventArgs e)
        {
            switch (sender.Name)
            {
                case "Console_MapBox":
                    TSMMain.Settings.World = ((MapData)sender.SelectedItem).Path;
                    break;
                default:
                    break;
            }
        }
        public override void OnListBoxSelectChange(ListBox sender, SelectionChangedEventArgs e)
        {
            switch (sender.Name)
            {
                case "Console_PlayerList":
                    if (sender.SelectedItem is null)
                        return;
                    if (sender.SelectedItem is PlayerInfo info && Info.Players.Contains(info))
                    {
                        PlayerManager.ChangeDisplayInfo(info);
                        TSMMain.GUI.MainTab.SelectedIndex = 2;
                        TSMMain.GUI.Tab_PlayerManage.SelectedIndex = 0;
                    }
                    else
                        Utils.Notice("此玩家尚未注册", HandyControl.Data.InfoType.Info);
                    sender.SelectedItem = null;
                    break;
                default:
                    break;
            }
        }
        public override void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var t = sender as System.Windows.Controls.TextBox;
                if (t.Name == "Console_CommandBox")
                {
                    if (!Info.IsServerRunning)
                        return;
                    Info.Server.AppendText(t.Text);
                    t.Clear();
                }
            }
        }
    }
}
