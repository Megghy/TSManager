using System.Windows;
using System.Windows.Controls.Primitives;

namespace TSManager.UI.Events
{
    internal class ESetting : GUIEvents.GUIEventBase
    {
        public override string ControlPrefix { get; } = "Setting";
        public override void OnSwichClick(ToggleButton sender, RoutedEventArgs e)
        {
            switch (sender.Name)
            {
                case "Setting_EnableDarkMode":
                    TSMMain.Settings.EnableDarkMode = (bool)sender.IsChecked;
                    TSMMain.Settings.Save();
                    TSMMain.GUI.ChangeNightMode((bool)sender.IsChecked ? HandyControl.Data.SkinType.Dark : HandyControl.Data.SkinType.Default);
                    break;
                default:
                    break;
            }
        }
    }
}
