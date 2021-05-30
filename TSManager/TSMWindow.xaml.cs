using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Themes;
using HandyControl.Tools;
using TShockAPI;
using TSManager.Data;
using TSManager.Modules;
using TSManager.UI.Control;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
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
        public bool forceClose = false;
        public bool restart = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Info.IsEnterWorld)
            {
                e.Cancel = true;
                if (forceClose) Environment.Exit(0);
                Growl.Ask("服务器正在运行, 确定要关闭并退出吗?", result =>
                {
                    if (result)
                    {
                        TSMMain.Instance.Stop();
                        Console_RestartServer.IsEnabled = false;
                        forceClose = true;
                    }
                    return true;
                });
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var b = sender as Button;
                var plrInfo = PlayerManage_List.SelectedItem as PlayerInfo;
                if (b.Name == "GoToStartServer")
                {
                    MainTab.SelectedIndex = 1;
                }
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
                                        TSMMain.Instance.Stop();
                                        restart = true;
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
                            Info.Server.AppendText(Console_CommandBox.Text);
                            Console_CommandBox.Clear();
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
                    if (b.Name == "BagManage_Refresh") { BagManager.Refresh(); return; }
                    if (b.DataContext is not ItemData item  || plrInfo == null)
                    {
                        Utils.Notice($"未选择物品", HandyControl.Data.InfoType.Warning);
                        return;
                    }
                    if (TShock.Utils.GetItemById(plrInfo.Data.inventory[item.Slot].NetId).type != item.ID)
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
                }
                else if (b.Name.StartsWith("GroupManage"))
                {
                    var group = GroupManage_List.SelectedItem as GroupData;
                    if (TShock.Groups.GetGroupByName(group.Name) == null)
                    {
                        Utils.Notice($"当前所选的用户组 {group.Name} 已不存在, 请尝试刷新", HandyControl.Data.InfoType.Warning);
                        return;
                    }
                    switch (b.Name)
                    {
                        case "GroupManage_DelPermission":
                            GroupManager.DelPermission(group, b.DataContext as PermissionData);
                            break;
                        case "GroupManage_AddPermission":
                            GroupManager.AddPermission(group, b.DataContext as PermissionData);
                            break;
                        case "GroupManage_Refresh":
                            GroupManager.RefreshGroupData();
                            break;
                        case "GroupManage_Save":
                            GroupManager.Save();
                            break;
                        case "GroupManage_Del":
                            GroupManager.Del(group);
                            break;
                    }
                }
            }
            catch (Exception ex) { Utils.Notice(ex, HandyControl.Data.InfoType.Error); }
        }
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
                else if(b.Name.StartsWith("GroupManage"))
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
            catch (Exception ex) { Utils.Notice(ex, HandyControl.Data.InfoType.Error); }
        }
        private void OnSwichClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var b = sender as ToggleButton;
                if (b.Name.StartsWith("CongifEditor"))
                {
                    #region 设置编辑器
                    switch (b.Name)
                    {
                    }
                    #endregion
                }
                else if (b.Name.StartsWith("PlayerManage"))
                {
                    #region 玩家管理器
                    if (b.DataContext is not Data.PlayerInfo plrInfo)
                    {
                        Utils.Notice("未选择玩家", HandyControl.Data.InfoType.Warning);
                        return;
                    }
                    if (!plrInfo.Online && b.Name is not "PlayerManage_Del" or "PlayerManage_Ban" or "PlayerManage_UnBan")
                    {
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
            }
            catch (Exception ex) { Utils.Notice(ex, HandyControl.Data.InfoType.Error); }
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
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
                            Info.Server.AppendText(t.Text);
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
        private void OnComboSelect(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var c = sender as ComboBox;
                switch (c.Name)
                {
                    case "ConfigEditor_List":
                        ConfigEdit.ChangeConfig((Data.ConfigData)c.SelectedItem);
                        break;
                    case "MapBox":
                        TSMMain.Settings.World = ((Data.Maps)Console_MapBox.SelectedItem).Path;
                        break;
                    case "PlayerManage_Group":
                        (PlayerManage_List.SelectedItem as Data.PlayerInfo).ChangeGroup(PlayerManage_Group.SelectedItem);
                        break;
                    case "Bag_Choose":
                        BagManager.ChangeBag(PlayerManage_List.SelectedItem as Data.PlayerInfo, (BagManager.BagType)c.SelectedValue);
                        break;
                    case "Bag_Prefix":
                        if (c.DataContext is ItemData data) BagManager.ChangePrefix(data, c.SelectedIndex);
                        break;
                }
            }
            catch (Exception ex) { Utils.Notice(ex, HandyControl.Data.InfoType.Error); }
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
                            var info = Info.Players.SingleOrDefault(p => p.Name == ((TSPlayer)l.SelectedItem).Name);
                            if (info != null) PlayerManager.ChangeDisplayInfo(info);
                            else Utils.Notice("此玩家尚未注册", HandyControl.Data.InfoType.Info);
                            break;
                        case "GroupManage_List":
                            GroupManager.ChangeSource((Data.GroupData)l.SelectedItem);
                            break;
                    }
                }
            }
            catch (Exception ex) { Utils.Notice(ex, HandyControl.Data.InfoType.Error); }
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
            { //Utils.Notice(ex, HandyControl.Data.InfoType.Error);
            }
        }
        internal void ChangeNightMode(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            SharedResourceDictionary.SharedDictionaries.Clear();
            TSMMain.Settings.EnableDarkMode = (bool)checkbox.IsChecked;
            if (TSMMain.Settings.EnableDarkMode) LOGO.Effect = new HandyControl.Media.Effects.ColorComplementEffect();
            else LOGO.Effect = null;
            ResourceHelper.GetTheme("HandyTheme", Application.Current.Resources).Skin = TSMMain.Settings.EnableDarkMode ? HandyControl.Data.SkinType.Dark : HandyControl.Data.SkinType.Default;
            OnApplyTemplate();
        }
    }
}
