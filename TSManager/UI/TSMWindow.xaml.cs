using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Themes;
using HandyControl.Tools;
using ScratchNet;
using TSManager.Data;
using TSManager.Modules;
using TSManager.UI.Control;
using Button = System.Windows.Controls.Button;
using ListView = System.Windows.Controls.ListView;

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
            ResizeMode = ResizeMode.CanMinimize;
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
            if ((sender as Button).Name == "GoToStartServer")
                MainTab.SelectedIndex = 1;
            UI.GUIEvents.OnGUIEvent(sender, e, UI.GUIEvents.EventType.ButtonClick);
        }
        private void OnButtonTextBoxClick(object sender, RoutedEventArgs e)
        {
            UI.GUIEvents.OnGUIEvent(sender, e, UI.GUIEvents.EventType.ButtonTextBoxClick);
        }
        private void OnSwichClick(object sender, RoutedEventArgs e)
        {
            UI.GUIEvents.OnGUIEvent(sender, e, UI.GUIEvents.EventType.SwitchClick);
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            UI.GUIEvents.OnGUIEvent(sender, e, UI.GUIEvents.EventType.KeyDown);
            if (e.Key == Key.Enter && sender is ButtonTextBox b)
            {
                b.Text = "";
                b.CallOnClick();
            }
        }
        private void OnComboSelect(object sender, SelectionChangedEventArgs e)
        {
            UI.GUIEvents.OnGUIEvent(sender, e, UI.GUIEvents.EventType.ComboSelectChange);
        }
        private void OnListSelect(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView)
                UI.GUIEvents.OnGUIEvent(sender, e, UI.GUIEvents.EventType.ListViewSelect);
            else if (sender is ListBox)
                UI.GUIEvents.OnGUIEvent(sender, e, UI.GUIEvents.EventType.ListBoxSelect);
            else if (sender is DataGrid)
                UI.GUIEvents.OnGUIEvent(sender, e, UI.GUIEvents.EventType.DataGridSelectChange);
        }
        private void OnEditorTextChange(object sender, EventArgs e) => ConfigEdit.OnTextChange(ConfigEditor.DataContext as ConfigData);
        private void OnTextInput(object sender, EventArgs e)
        {
            UI.GUIEvents.OnGUIEvent(sender, e, UI.GUIEvents.EventType.TextInput);
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is GraphicScriptEditor)
            {
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
            UI.GUIEvents.OnGUIEvent(sender, e, UI.GUIEvents.EventType.SelectedColorChange);
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void HyperlinkClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(sender.ToType<Hyperlink>().NavigateUri.AbsoluteUri));
        }
    }
}
