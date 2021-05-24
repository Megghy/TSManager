using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using HandyControl.Controls;
using TSManager.Modules;
using TSManager.UI.Control;
using ComboBox = HandyControl.Controls.ComboBox;
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
                if (b.Name.StartsWith("Console"))
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
                    if (b.DataContext is not Data.PlayerInfo plrInfo)
                    {
                        Utils.Notice("未选择玩家", HandyControl.Data.InfoType.Error);
                        return;
                    }
                    if (!plrInfo.Online && b.Name is not "PlayerManage_Del" or "PlayerManage_Ban" or "PlayerManage_UnBan")
                    {
                        Utils.Notice("玩家 " + plrInfo.Name + " 未在线.", HandyControl.Data.InfoType.Error);
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
                            break;

                    }
                    #endregion
                }
            }
            catch (Exception ex) { Utils.Notice(ex, HandyControl.Data.InfoType.Error); }
        }
        private void OnButtonTextBoxClick(object sender, RoutedEventArgs e)
        {
            var b = sender as ButtonTextBox;
            if (b.Name.StartsWith("PlayerManage"))
            {
                #region 玩家管理器
                if (b.DataContext is not Data.PlayerInfo plrInfo)
                {
                    Utils.Notice("未选择玩家", HandyControl.Data.InfoType.Error);
                    return;
                }
                if (!plrInfo.Online && b.Name is not "PlayerManage_Del" or "PlayerManage_Ban" or "PlayerManage_UnBan")
                {
                    Utils.Notice("玩家 " + plrInfo.Name + " 未在线.", HandyControl.Data.InfoType.Error);
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
                        else Utils.Notice("无效的伤害数值", HandyControl.Data.InfoType.Error);
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
        }
        private void OnSwichClick(object sender, RoutedEventArgs e)
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
                    Utils.Notice("未选择玩家", HandyControl.Data.InfoType.Error);
                    return;
                }
                if (!plrInfo.Online && b.Name is not "PlayerManage_Del" or "PlayerManage_Ban" or "PlayerManage_UnBan")
                {
                    Utils.Notice("玩家 " + plrInfo.Name + " 未在线.", HandyControl.Data.InfoType.Error);
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
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (sender is TextBox)
                    {
                        var t = sender as TextBox;
                        if (t.Name == "CommandBox")
                        {
                            if (!Info.IsServerRunning) return;
                            Info.Server.AppendText(t.Text);
                            t.Clear();
                        }
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
                        if (c.DataContext is Data.ItemData) BagManager.ChangePrefix(((Data.ItemData)c.DataContext), (int)c.SelectedValue);
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
                            PlayerManager.ChangeDisplayInfo((Data.PlayerInfo)l.SelectedItem);
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

        
    }
}
