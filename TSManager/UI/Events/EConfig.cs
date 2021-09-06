using System.Windows;
using System.Windows.Controls;
using TSManager.Data;
using TSManager.Modules;
using ComboBox = HandyControl.Controls.ComboBox;

namespace TSManager.UI.Events
{
    internal class EConfig : GUIEvents.GUIEventBase
    {
        public override string ControlPrefix => "ConfigEditor";
        public override void OnButtonClick(Button sender, RoutedEventArgs e)
        {
            switch (sender.Name)
            {
                case "ConfigEditor_Format":
                    ConfigEdit.Format(sender.DataContext.ToType<ConfigData>());
                    break;
                case "ConfigEditor_Refresh":
                    ConfigEdit.LoadAllConfig();
                    break;
                case "ConfigEditor_Save":
                    ConfigEdit.Save(sender.DataContext.ToType<ConfigData>());
                    break;
                case "ConfigEditor_OpenFile":
                    ConfigEdit.OpenFile(sender.DataContext.ToType<ConfigData>());
                    break;
                default:
                    break;
            }
        }
        public override void OnComboSelectChange(ComboBox sender, SelectionChangedEventArgs e)
        {
            switch (sender.Name)
            {
                case "ConfigEditor_List":
                    ConfigEdit.ChangeConfig(ConfigEdit.SelectedConfigData);
                    break;
                default:
                    break;
            }
        }
    }
}
