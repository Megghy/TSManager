using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HandyControl.Controls;
using HandyControl.Data;
using TSManager.Data;
using TSManager.Modules;
using TSManager.UI.Control;

namespace TSManager.UI.Events
{
    internal class EGroup : GUIEvents.GUIEventBase
    {
        public override string ControlPrefix => "GroupManage";
        public override void OnButtonClick(Button sender, RoutedEventArgs e)
        {
            var group = GroupManager.SelectedGroupData;
            switch (sender.Name)
            {
                case "GroupManage_DelPermission":
                    GroupManager.DelPermission(group, sender.DataContext as PermissionData);
                    break;
                case "GroupManage_AddPermission":
                    GroupManager.AddPermission(group, sender.DataContext as PermissionData);
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
                default:
                    break;
            }
        }
        public override void OnButtonTextBoxClick(ButtonTextBox sender, RoutedEventArgs e)
        {
            switch (sender.Name)
            {
                case "GroupManage_Create":
                    GroupManager.Create(sender.Text);
                    sender.Text = "";
                    break;
                case "GroupManage_AddManually":
                    GroupManager.AddManually(GroupManager.SelectedGroupData, sender.Text);
                    sender.Text = "";
                    break;
                default:
                    break;
            }
        }
        public override void OnListBoxSelectChange(ListBox sender, SelectionChangedEventArgs e)
        {
            switch (sender.Name)
            {
                case "GroupManage_List":
                    GroupManager.ChangeSource(GroupManager.SelectedGroupData);
                    break;
                default:
                    break;
            }
        }
        public override void OnSelectedColorChanged(ColorPicker sender, FunctionEventArgs<Color> e)
        {
            if (GroupManager.SelectedGroupData != null)
            {
                TSMMain.GUI.GroupManager_Drawer.IsOpen = false;
                GroupManager.SelectedGroupData.ChatColor = e.Info;
            }
        }
    }
}
