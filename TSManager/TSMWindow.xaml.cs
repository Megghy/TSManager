using HandyControl.Controls;
using HandyControl.Themes;
using HandyControl.Tools;
using ScratchNet;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using TShockAPI;
using TSManager.Data;
using TSManager.Modules;
using TSManager.UI.Control;
using Button = System.Windows.Controls.Button;
using ComboBox = HandyControl.Controls.ComboBox;
using ListView = System.Windows.Controls.ListView;
using PasswordBox = HandyControl.Controls.PasswordBox;
using TextBox = HandyControl.Controls.TextBox;

namespace TSManager
{
    /// <summary>
    /// TSMWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TSMWindow : HandyControl.Controls.Window
    {
        public TSMWindow()
        {
            InitializeComponent();
            TSMMain.GUI = this;
        }
        public static TSMWindow Instance;

        private void Window_Loaded(object sender, EventArgs e)
        {
            Instance = this;
            TSMMain.Instance.OnInitialize();
        }
        internal bool forceClose = false;
        internal bool reStart = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Info.IsEnterWorld)
            {
                e.Cancel = true;
                if (forceClose)
                    Environment.Exit(0);
                Growl.Ask("服务器正在运行, 确定要关闭并退出吗?", result =>
                {
                    if (result)
                    {
                        TSMMain.Instance.StopServer();
                        Console_RestartServer.IsEnabled = false;
                        forceClose = true;
                    }
                    return true;
                });
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var b = sender as Button;
                var plrInfo = PlayerManage_List.SelectedItem as PlayerInfo;
                if (b.Name == "GoToStartServer") MainTab.SelectedIndex = 1;
                else if (b.Name.StartsWith("Console"))
                {
                    #region 控制台
                    switch (b.Name)
                    {
                        case "Console_StartServer":
                            Info.Server.Start(Info.Server.GetStartArgs());
                            break;
                        case "Console_StopServer":
                            Close();
                            break;
                        case "Console_RestartServer":
                            if (Info.IsServerRunning)
                            {
                                Growl.Ask("服务器正在运行, 确定要关闭并重启吗?", result =>
                                {
                                    if (result)
                                    {
                                        reStart = true;
                                        TSMMain.Instance.StopServer();
                                    }
                                    return true;
                                });
                            }
                            else
                            {
                                Visibility = Visibility.Hidden;
                                Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
                                Environment.Exit(0);
                            }
                            break;
                        case "Console_DoCmd":
                            if (!Info.IsServerRunning) return;
                            Info.Server.ExcuteConsoleCommand(Console_CommandBox.Text);
                            Console_CommandBox.Clear();
                            break;
                        case "Console_Clear":
                            Console_ConsoleBox.Document.Blocks.Clear();
                            Utils.Notice("已清空控制台文本", HandyControl.Data.InfoType.Success);
                            break;
                    }
                    #endregion
                }
                if (b.Name.StartsWith("ConfigEditor"))
                {
                    #region 设置编辑器
                    switch (b.Name)
                    {

                        case "ConfigEditor_Format":
                            ConfigEdit.Format((Data.ConfigData)b.DataContext);
                            break;
                        case "ConfigEditor_Refresh":
                            ConfigEdit.LoadAllConfig();
                            break;
                        case "ConfigEditor_Save":
                            ConfigEdit.Save((Data.ConfigData)b.DataContext);
                            break;
                        case "ConfigEditor_OpenFile":
                            ConfigEdit.OpenFile((Data.ConfigData)b.DataContext);
                            break;

                    }
                    #endregion
                }
                else if (b.Name.StartsWith("PlayerManage"))
                {
                    #region 玩家管理器
                    if (plrInfo == null)
                    {
                        Utils.Notice("未选择玩家", HandyControl.Data.InfoType.Warning);
                        return;
                    }
                    if (!plrInfo.Online && b.Name != "PlayerManage_Del" && b.Name != "PlayerManage_UnBan")
                    {
                        Utils.Notice("玩家 " + plrInfo.Name + " 未在线.", HandyControl.Data.InfoType.Warning);
                        return;
                    }
                    switch (b.Name)
                    {

                        case "PlayerManage_Heal":
                            plrInfo.Heal();
                            break;
                        case "PlayerManage_Kill":
                            plrInfo.Kill();
                            break;
                        case "PlayerManage_Del":
                            Growl.Ask($"确定要删除玩家 {plrInfo} 的账号吗?", result =>
                            {
                                if (result) plrInfo.Del();
                                return true;
                            });
                            break;
                        case "PlayerManage_UnBan":
                            plrInfo.UnBan();
                            break;
                    }
                    #endregion
                }
                else if (b.Name.StartsWith("BagManage"))
                {
                    #region 背包管理器
                    if (b.Name == "BagManage_Refresh") { BagManager.Refresh(); return; }
                    if (b.DataContext is not ItemData item || plrInfo == null)
                    {
                        Utils.Notice($"未选择物品", HandyControl.Data.InfoType.Warning);
                        return;
                    }
                    if (CheckItemSlot(plrInfo, item))
                    {
                        Utils.Notice("此物品所在位置已变动, 请刷新");
                        return;
                    }
                    switch (b.Name)
                    {
                        case "BagManage_ChangeStack":
                            BagManager.ChangeStack(item);
                            break;
                        case "BagManage_Del":
                            BagManager.DelItem(item);
                            break;
                    }
                    #endregion
                }
                else if (b.Name.StartsWith("GroupManage"))
                {
                    #region 用户组管理器
                    var group = GroupManage_List.SelectedItem as GroupData;
                    switch (b.Name)
                    {
                        case "GroupManage_DelPermission":
                            GroupManager.DelPermission(group, b.DataContext as PermissionData);
                            break;
                        case "GroupManage_AddPermission":
                            GroupManager.AddPermission(group, b.DataContext as PermissionData);
                            break;
                        case "GroupManage_Refresh":
                            GroupManager.Refresh();
                            Utils.Notice("已刷新用户组数据", HandyControl.Data.InfoType.Success);
                            break;
                        case "GroupManage_Save":
                            GroupManager.Save();
                            break;
                        case "GroupManage_Del":
                            GroupManager.Del(group);
                            break;
                    }
                    #endregion
                }
                else if (b.Name.StartsWith("Script"))
                {
                    #region 脚本管理器
                    switch (b.Name)
                    {
                        case "Script_Confirm":
                            ScriptManager.Create(Script_Create_Name.Text, Script_Create_Author.Text, Script_Create_Description.Text, Script_Create_Version.Text);
                            break;
                        case "Script_Cancel":
                            Script_Create_Drawer.IsOpen = false;
                            break;
                        case "Script_GoToEdit":
                            if (Script_List.SelectedItem is { } script) ScriptManager.ChangeSelectScript(Script_List.SelectedItem as ScriptData);
                            else Utils.Notice("未选择脚本");
                            break;
                        case "Script_Save":
                            ScriptManager.Save(Script_Editor.Script as ScriptData);
                            break;
                        case "Script_Del":
                            if (Script_List.SelectedItem is { } script_Del) Growl.Ask($"确定要删除脚本 {((ScriptData)script_Del).Name} 吗?", result =>
                            {
                                if (result)
                                    ScriptManager.Del(Script_List.SelectedItem as ScriptData);
                                return true;
                            });
                            else
                                Utils.Notice("未选择脚本");
                            break;
                        case "Script_Paste":
                            Script_Editor.Paste(new(10, 10));
                            break;
                        case "Script_Copy":
                            Script_Editor.Copy();
                            break;
                    }
                    #endregion
                }
                else if (b.Name.StartsWith("Download"))
                {
                    #region 下载管理器
                    switch (b.Name)
                    {
                    }
                    #endregion
                }
            }
            catch (Exception ex) { ex.ShowError(); }
        }
        bool CheckItemSlot(PlayerInfo plrInfo, ItemData item) => TShock.Utils.GetItemById(plrInfo.Data.inventory[item.Slot].NetId).type != item.ID;
        private void OnButtonTextBoxClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var b = sender as ButtonTextBox;
                if (b.Name.StartsWith("PlayerManage"))
                {
                    #region 玩家管理器
                    if (b.DataContext is not Data.PlayerInfo plrInfo)
                    {
                        Utils.Notice("未选择玩家", HandyControl.Data.InfoType.Warning);
                        return;
                    }
                    if (!plrInfo.Online && b.Name != "PlayerManage_Ban" && b.Name != "PlayerManage_Password")
                    {
                        Utils.Notice("玩家 " + plrInfo.Name + " 未在线.", HandyControl.Data.InfoType.Warning);
                        return;
                    }
                    switch (b.Name)
                    {
                        case "PlayerManage_Password":
                            if (plrInfo.ChangePassword(b.Text)) b.Text = "";
                            break;
                        case "PlayerManage_Kick":
                            plrInfo.Kick(b.Text);
                            break;
                        case "PlayerManage_Ban":
                            plrInfo.AddBan(b.Text);
                            break;
                        case "PlayerManage_UnBan":
                            plrInfo.UnBan();
                            break;
                        case "PlayerManage_Whisper":
                            plrInfo.Whisper(b.Text);
                            break;
                        case "PlayerManage_Damage":
                            if (int.TryParse(b.Text, out int damage))
                            {
                                plrInfo.Damage(damage);
                            }
                            else Utils.Notice("无效的伤害数值", HandyControl.Data.InfoType.Warning);
                            break;
                        case "PlayerManage_Command":
                            plrInfo.Command(b.Text);
                            break;
                        case "PlayerManage_GodMode":
                            plrInfo.GodMode();
                            break;
                        case "PlayerManage_Mute":
                            plrInfo.Mute();
                            break;
                    }
                    #endregion
                }
                else if (b.Name.StartsWith("GroupManage"))
                {
                    #region 用户组管理器
                    var group = GroupManage_List.SelectedItem as GroupData;
                    switch (b.Name)
                    {
                        case "GroupManage_Create":
                            GroupManager.Create(GroupManage_Create.Text);
                            GroupManage_Create.Text = "";
                            break;
                        case "GroupManage_AddManually":
                            GroupManager.AddManually(group, GroupManage_AddManually.Text);
                            GroupManage_AddManually.Text = "";
                            break;
                    }
                    #endregion
                }
            }
            catch (Exception ex) { ex.ShowError(); }
        }
        private void OnSwichClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var b = sender as ToggleButton;
                if (b.Name.StartsWith("CongifEditor"))
                {
                    #region 设置编辑器
                    #endregion
                }
                else if (b.Name.StartsWith("PlayerManage"))
                {
                    #region 玩家管理器
                    if (b.DataContext is not PlayerInfo plrInfo)
                    {
                        Utils.Notice("未选择玩家", HandyControl.Data.InfoType.Warning);
                        return;
                    }
                    if (!plrInfo.Online && b.Name is not "PlayerManage_Del" or "PlayerManage_Ban" or "PlayerManage_UnBan")
                    {
                        b.IsChecked = false;
                        Utils.Notice("玩家 " + plrInfo.Name + " 未在线.", HandyControl.Data.InfoType.Warning);
                        return;
                    }
                    switch (b.Name)
                    {
                        case "PlayerManage_GodMode":
                            plrInfo.GodMode();
                            break;
                        case "PlayerManage_Mute":
                            plrInfo.Mute();
                            break;
                    }
                    #endregion
                }
                else if (b.Name.StartsWith("Console"))
                {
                    switch (b.Name)
                    {
                        case "Console_AutoStart":
                            TSMMain.Settings.AutoStart = (bool)b.IsChecked;
                            break;
                        case "Console_EnableChinese":
                            TSMMain.Settings.EnableChinese = (bool)b.IsChecked;
                            break;
                    }
                    TSMMain.Settings.Save();
                }
                else if (b.Name.StartsWith("Setting"))
                {
                    switch (b.Name)
                    {
                        case "Setting_EnableDarkMode":
                            TSMMain.Settings.EnableDarkMode = (bool)b.IsChecked;
                            TSMMain.Settings.Save();
                            ChangeNightMode((bool)b.IsChecked ? HandyControl.Data.SkinType.Dark : HandyControl.Data.SkinType.Default);
                            break;
                    }
                }
            }
            catch (Exception ex) { ex.ShowError(); }
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (sender is GraphicScriptEditor)
                {
                    var s = sender as GraphicScriptEditor;
                    switch (e.Key)
                    {
                        case Key.C:
                            s.Copy();
                            break;
                        case Key.V:
                            VisualBox.Focus();
                            s.Paste(Mouse.GetPosition(s));
                            break;
                    }

                }

            }
            else
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        if (sender is TextBox)
                        {
                            var t = sender as TextBox;
                            if (t.Name == "Console_CommandBox")
                            {
                                if (!Info.IsServerRunning) return;
                                Info.Server.ExcuteConsoleCommand(t.Text);
                                t.Clear();
                            }
                        }
                        else if (sender is ButtonTextBox)
                        {
                            var b = sender as ButtonTextBox;
                            b.Text = "";
                            b.CallOnClick();
                        }
                        break;
                }
            }
        }
        private void OnComboSelect(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var c = sender as ComboBox;
                switch (c.Name)
                {
                    case "ConfigEditor_List":
                        ConfigEdit.ChangeConfig((ConfigData)c.SelectedItem);
                        break;
                    case "Console_MapBox":
                        TSMMain.Settings.World = ((MapData)Console_MapBox.SelectedItem).Path;
                        break;
                    case "PlayerManage_Group":
                        (PlayerManage_List.SelectedItem as PlayerInfo).ChangeGroup(PlayerManage_Group.SelectedItem);
                        break;
                    case "Bag_Choose":
                        BagManager.ChangeBag(PlayerManage_List.SelectedItem as Data.PlayerInfo, (BagManager.BagType)c.SelectedValue);
                        break;
                    case "Bag_Prefix":
                        if (c.DataContext is ItemData data) BagManager.ChangePrefix(data, c.SelectedIndex);
                        break;
                    case "Script_TriggerCondition":
                        if (Script_List.SelectedItem is { } script && ((ScriptData)script).TriggerCondition != (ScriptData.Triggers)Script_TriggerCondition.SelectedItem)
                        {
                            ScriptManager.ChangeTriggerCondition((ScriptData)script, (ScriptData.Triggers)Script_TriggerCondition.SelectedItem);
                        }
                        break;
                }
            }
            catch (Exception ex) { ex.ShowError(); }
        }
        private void OnListSelect(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is ListView)
                {
                    var l = sender as ListView;
                    switch (l.Name)
                    {
                        case "Download_TShock_List":
                            Downloader.SelectTSFile((DownloadInfo.TShockInfo)l.SelectedItem);
                            break;
                    }
                }
                else if (sender is ListBox)
                {
                    var l = sender as ListBox;
                    switch (l.Name)
                    {
                        case "PlayerManage_List":
                            PlayerManager.ChangeDisplayInfo((PlayerInfo)l.SelectedItem);
                            break;
                        case "Console_PlayerList":
                            if (l.SelectedItem is null) return;
                            if (Utils.TryGetPlayerInfo((l.SelectedItem as PlayerInfo).Name, out var info))
                            {
                                PlayerManager.ChangeDisplayInfo(info);
                                MainTab.SelectedIndex = 2;
                                Tab_PlayerManage.SelectedIndex = 0;
                            }
                            else
                                Utils.Notice("此玩家尚未注册", HandyControl.Data.InfoType.Info);
                            l.SelectedItem = null;
                            break;
                        case "GroupManage_List":
                            GroupManager.ChangeSource((GroupData)l.SelectedItem);
                            break;
                        case "Script_List":
                            ScriptManager.ChangeSelectScript((ScriptData)l.SelectedItem);
                            break;
                    }
                }
                else if (sender is DataGrid)
                {
                    var d = sender as DataGrid;
                    switch (d.Name)
                    {
                        case "Script_List":
                            ScriptManager.ChangeSelectScript(d.SelectedItem as ScriptData);
                            break;
                    }
                }
            }
            catch (Exception ex) { ex.ShowError(); }
        }
        private void OnEditorTextChange(object sender, EventArgs e) => ConfigEdit.OnTextChange(ConfigEditor.DataContext as Data.ConfigData);
        private void OnTextInput(object sender, EventArgs e)
        {
            try
            {
                if (sender is TextBox)
                {
                    var t = sender as TextBox;
                    switch (t.Name)
                    {
                        case "MaxPlayerBox":
                            TSMMain.Settings.MaxPlayer = int.Parse(Console_MaxPlayerBox.Text);
                            break;
                        case "PortBox":
                            TSMMain.Settings.Port = int.Parse(Console_PortBox.Text);
                            break;
                    }
                }
                else if (sender is PasswordBox)
                {
                    var p = sender as PasswordBox;
                    switch (p.Name)
                    {
                        case "PasswordBox":
                            TSMMain.Settings.Password = p.Password;
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                ex.ShowError();
            }
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is GraphicScriptEditor)
            {
                //var s = sender as GraphicScriptEditor;
                VisualBox.Focus();
            }
        }
        internal void ChangeNightMode(HandyControl.Data.SkinType type)
        {
            if (type == HandyControl.Data.SkinType.Dark)
                LOGO.Effect = new HandyControl.Media.Effects.ColorComplementEffect();
            else
                LOGO.Effect = null;
            SharedResourceDictionary.SharedDictionaries.Clear();
            Resources.MergedDictionaries.Add(ResourceHelper.GetSkin(type));
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
            });
            OnApplyTemplate();
        }
        private void GroupManage_ChatColor_Canceled(object sender, EventArgs e) => GroupManager_Drawer.IsOpen = false;
        private void GroupManage_ChatColor_SelectedColorChanged(object sender, HandyControl.Data.FunctionEventArgs<System.Windows.Media.Color> e)
        {
            var group = (sender as ColorPicker).DataContext as GroupData;
            GroupManager_Drawer.IsOpen = false;
            group.ChatColor = e.Info;
        }
    }
}
