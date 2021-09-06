using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HandyControl.Controls;
using ScratchNet;
using TSManager.Data;
using static TSManager.TSMMain;
using ComboBox = HandyControl.Controls.ComboBox;

namespace TSManager.UI.Events
{
    internal class EScript : GUIEvents.GUIEventBase
    {
        public override string ControlPrefix { get; } = "Script";
        public override void OnButtonClick(Button sender, RoutedEventArgs e)
        {
            switch (sender.Name)
            {
                case "Script_Confirm":
                    ScriptManager.Create(GUI.Script_Create_Name.Text, GUI.Script_Create_Author.Text, GUI.Script_Create_Description.Text, GUI.Script_Create_Version.Text);
                    break;
                case "Script_Cancel":
                    GUI.Script_Create_Drawer.IsOpen = false;
                    break;
                case "Script_GoToEdit":
                    if (ScriptManager.SelectedScriptData is { })
                        ScriptManager.ChangeSelectScript(ScriptManager.SelectedScriptData);
                    else
                        Utils.Notice("未选择脚本");
                    break;
                case "Script_Save":
                    ScriptManager.Save(GUI.Script_Editor.Script as ScriptData);
                    break;
                case "Script_Del":
                    if (ScriptManager.SelectedScriptData is { })
                        Growl.Ask($"确定要删除脚本 {ScriptManager.SelectedScriptData.Name} 吗?", result =>
                        {
                            if (result)
                                ScriptManager.Del(GUI.Script_List.SelectedItem as ScriptData);
                            return true;
                        });
                    else
                        Utils.Notice("未选择脚本");
                    break;
                case "Script_Paste":
                    GUI.Script_Editor.Paste(new(10, 10));
                    break;
                case "Script_Copy":
                    GUI.Script_Editor.Copy();
                    break;
            }
        }
        public override void OnComboSelectChange(ComboBox sender, SelectionChangedEventArgs e)
        {
            switch (sender.Name)
            {
                case "Script_TriggerCondition":
                    if (ScriptManager.SelectedScriptData is { } && ScriptManager.SelectedScriptData.TriggerCondition != (ScriptData.Triggers)GUI.Script_TriggerCondition.SelectedItem)
                    {
                        ScriptManager.ChangeTriggerCondition(ScriptManager.SelectedScriptData, (ScriptData.Triggers)GUI.Script_TriggerCondition.SelectedItem);
                    }
                    break;
                default:
                    break;
            }
        }
        public override void OnDataGridSelectChange(DataGrid sender, SelectionChangedEventArgs e)
        {
            switch (sender.Name)
            {
                case "Script_List":
                    ScriptManager.ChangeSelectScript(ScriptManager.SelectedScriptData);
                    break;
                default:
                    break;
            }
        }
        public override void OnKeyDown(object sender, KeyEventArgs e)
        {
            var s = sender as GraphicScriptEditor;
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.C:
                        s.Copy();
                        break;
                    case Key.V:
                        GUI.VisualBox.Focus();
                        s.Paste(Mouse.GetPosition(s));
                        break;
                }
            }
        }
    }
}
