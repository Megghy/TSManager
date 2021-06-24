using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TShockAPI;
using TSManager.Data;

namespace TSManager.Modules
{
    class GroupManager
    {
        public async static void RefreshGroupData()
        {
            try
            {
                await Task.Run(() =>
                {
                    //读取所有用户组
                    var groups = new List<GroupData>();
                    TShock.Groups.ForEach(g => groups.Add(new GroupData(g, true)));
                    TSMMain.GUIInvoke(() =>
                    {
                        int index = TSMMain.GUI.GroupManage_List.SelectedIndex;
                        string groupname = TSMMain.GUI.GroupManage_List.SelectedItem == null ? null : (TSMMain.GUI.GroupManage_List.SelectedItem as GroupData).Name;
                        TSMMain.GUI.GroupManage_List.ItemsSource = null;
                        TSMMain.GUI.GroupManage_List.ItemsSource = new BindingList<GroupData>(groups);
                        //在最前面加一个表示没有父组的选项
                        var tempGroup = new List<GroupData>(groups);
                        tempGroup.Insert(0, new(true) { Name = "<无>" });
                        TSMMain.GUI.GroupManage_Parents.ItemsSource = null;
                        TSMMain.GUI.GroupManage_Parents.ItemsSource = tempGroup;
                        if (index != -1)
                        {
                            var list = groups.Where(g => g.Name == groupname && !g.NoParent).ToList();
                            if (list.Any()) ChangeSource(list[0]);
                            else if (groups.Count > 0) ChangeSource(groups[0]);
                        }
                        else if (groups.Count > 0) ChangeSource(groups[0]);
                    });
                });
            }
            catch { }
        }
        public static void Del(GroupData group)
        {
            if (TShock.Groups.GetGroupByName(group.Name) == null)
            {
                Utils.Notice($"用户组 {group.Name} 已不存在, 请尝试刷新", HandyControl.Data.InfoType.Warning);
                return;
            }
            TShock.Groups.DeleteGroup(group.Name);
            Utils.Notice($"已删除用户组 {group.Name}.", HandyControl.Data.InfoType.Success);
            RefreshGroupData();
        }
        public static void AddPermission(GroupData group, PermissionData permission)
        {
            if (TShock.Groups.GetGroupByName(group.Name) == null)
            {
                Utils.Notice($"当前所选的用户组 {group.Name} 已不存在, 请尝试刷新", HandyControl.Data.InfoType.Warning);
                return;
            }
            var tsGroup = TShock.Groups.GetGroupByName(group.Group.Name);
            if (!tsGroup.TotalPermissions.Contains(permission.Name))
            {
                tsGroup.AddPermission(permission.Name);
                group.Permissions.Add(permission);
                Utils.Notice($"已添加权限 {permission.Name}.", HandyControl.Data.InfoType.Success);
            }
            else
            {
                Utils.Notice($"此用户组已存在权限 {permission.Name}.", HandyControl.Data.InfoType.Error);
                return;
            }
            //RefreshGroupData();
        }
        public static void DelPermission(GroupData group, PermissionData permission)
        {
            if (TShock.Groups.GetGroupByName(group.Name) == null)
            {
                Utils.Notice($"当前所选的用户组 {group.Name} 已不存在, 请尝试刷新", HandyControl.Data.InfoType.Warning);
                return;
            }
            var tsGroup = TShock.Groups.GetGroupByName(group.Group.Name);
            if (tsGroup.permissions.Contains(permission.Name))
            {
                tsGroup.RemovePermission(permission.Name);
                group.Permissions.Remove(permission);
                Utils.Notice($"已删除权限 {permission.Name}.", HandyControl.Data.InfoType.Success);
            }
            else
            {
                Utils.Notice($"未能在用户组权限列表中找到 {permission.Name}.", HandyControl.Data.InfoType.Error);
                return;
            }
            //RefreshGroupData();
        }
        public static void ChangeSource(GroupData group)
        {
            TSMMain.GUIInvoke(() =>
            {
                if (group == null) return;
                TSMMain.GUI.GroupManage_Permission.ItemsSource = group.Permissions;
                TSMMain.GUI.GroupManage.DataContext = group;
                TSMMain.GUI.GroupManage_ChatColor.DataContext = group;
                TSMMain.GUI.GroupManage_ChatColor.SelectedBrush = (System.Windows.Media.SolidColorBrush)group.DisplayColor;

                for (int i = 0; i < TSMMain.GUI.GroupManage_List.Items.Count; i++)
                {
                    if (((GroupData)TSMMain.GUI.GroupManage_List.Items[i]).Name == group.Name) TSMMain.GUI.GroupManage_List.SelectedIndex = i;
                }
                for (int i = 0; i < TSMMain.GUI.GroupManage_List.Items.Count; i++)
                {
                    if (group.Parent == null)
                    {
                        TSMMain.GUI.GroupManage_Parents.SelectedIndex = 0;
                        break;
                    }
                    if (((GroupData)TSMMain.GUI.GroupManage_Parents.Items[i]).Name == group.Parent.Name) TSMMain.GUI.GroupManage_Parents.SelectedIndex = i;
                }
            });

        }
        public static void Save()
        {
            var group = TSMMain.GUI.GroupManage_List.SelectedItem as GroupData;
            if (group == null)
            {
                Utils.Notice("当前未选择用户组.", HandyControl.Data.InfoType.Error);
                return;
            }
            var tsGroup = TShock.Groups.GetGroupByName(group.Group.Name);
            if (tsGroup == null)
            {
                Utils.Notice($"用户组 {group.Group.Name} 已不存在, 请尝试刷新用户组列表.", HandyControl.Data.InfoType.Error);
                return;
            }
            if (TSMMain.GUI.GroupManage_Parents.Text == tsGroup.Name || TSMMain.GUI.GroupManage_Parents.Text == group.Name)
            {
                Utils.Notice($"请勿将用户组本身设为父组.", HandyControl.Data.InfoType.Error);
                return;
            }
            if (TSMMain.GUI.GroupManage_Name.Text != tsGroup.Name)
            {
                var renamegroup = TShock.Groups.GetGroupByName(TSMMain.GUI.GroupManage_Name.Text);
                if (renamegroup != null)
                {
                    Utils.Notice($"欲重命名到的用户组 {TSMMain.GUI.GroupManage_Name.Text} 已存在.", HandyControl.Data.InfoType.Error);
                    return;
                }
                else
                {
                    try
                    {
                        Utils.Notice($"已重命名用户组.\n.{TShock.Groups.RenameGroup(tsGroup.Name, TSMMain.GUI.GroupManage_Name.Text)};", HandyControl.Data.InfoType.Info);
                        group.Name = TSMMain.GUI.GroupManage_Name.Text;
                        tsGroup = TShock.Groups.GetGroupByName(TSMMain.GUI.GroupManage_Name.Text);
                    }
                    catch (Exception ex) { Utils.Notice($"未能重命名用户组.\n{ex.Message}", HandyControl.Data.InfoType.Error); return; }
                }
            }
            try
            {
                TShock.Groups.UpdateGroup(tsGroup.Name, ((GroupData)TSMMain.GUI.GroupManage_Parents.SelectedItem).NoParent ? "" : TSMMain.GUI.GroupManage_Parents.Text, string.Join(",", tsGroup.permissions), $"{group.ChatColor.R},{group.ChatColor.G},{group.ChatColor.B}", TSMMain.GUI.GroupManage_Suffix.Text, TSMMain.GUI.GroupManage_Prefix.Text); Utils.Notice($"已更新用户组.", HandyControl.Data.InfoType.Success);
                RefreshGroupData();
                foreach (GroupData g in TSMMain.GUI.GroupManage_List.Items)
                {
                    if (g.Name == tsGroup.Name) ChangeSource(g);
                }
            }
            catch (Exception ex) { Utils.Notice($"未能更新用户组.\n{ex.Message}", HandyControl.Data.InfoType.Error); }
        }
        public static void Create(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) || name == "")
                {
                    Utils.Notice($"请输入组名.", HandyControl.Data.InfoType.Error);
                    return;
                }
                TShock.Groups.AddGroup(name, "", "", "255,255,255");
                (TSMMain.GUI.GroupManage_List.ItemsSource as BindingList<GroupData>).Add(new(TShock.Groups.GetGroupByName(name)));
                foreach (GroupData g in TSMMain.GUI.GroupManage_List.ItemsSource)
                {
                    if (g.Name == name)
                    {
                        ChangeSource(g);
                        break;
                    }
                }
                TSMMain.GUI.GroupManage_Create.Text = "";
                Utils.Notice($"成功创建用户组 {name}.", HandyControl.Data.InfoType.Success);
            }
            catch (Exception ex)
            {
                Utils.Notice($"创建失败.\n{ex}", HandyControl.Data.InfoType.Error);
            }
        }
        public static void AddManually(GroupData group, string permission)
        {
            try
            {
                var tsGroup = TShock.Groups.GetGroupByName(group.Group.Name);
                if (tsGroup == null)
                {
                    Utils.Notice($"当前所选的用户组 {group.Group.Name} 已不存在, 请尝试刷新用户组列表.", HandyControl.Data.InfoType.Error);
                    return;
                }
                if (permission == "" || permission == " ")
                {
                    Utils.Notice($"请输入权限.", HandyControl.Data.InfoType.Error);
                    return;
                }
                AddPermission(group, new(permission));
            }
            catch (Exception ex) { Utils.Notice($"未能添加权限.\n{ex.Message}", HandyControl.Data.InfoType.Error); }
        }
        public static void Search()
        {
            TSMMain.GUIInvoke(() =>
            {
                try
                {
                    if (TSMMain.GUI.GroupManage_List.ItemsSource != null)
                    {
                        string text = TSMMain.GUI.GroupManage_Search.Text;
                        var list = ((List<GroupData>)TSMMain.GUI.GroupManage_List.ItemsSource).Where(p => p.Name.ToLower().Contains(text.ToLower())).ToList();
                        if (list.Any())
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                if ((GroupData)TSMMain.GUI.GroupManage_List.SelectedItem == list[i])
                                {
                                    if (list.Count > i + 1)
                                    {
                                        TSMMain.GUI.GroupManage_List.ScrollIntoView(list[i + 1]);
                                        TSMMain.GUI.GroupManage_List.SelectedItem = list[i + 1];
                                    }
                                    else
                                    {
                                        TSMMain.GUI.GroupManage_List.ScrollIntoView(list[0]);
                                        TSMMain.GUI.GroupManage_List.SelectedItem = list[0];
                                    }
                                    return;
                                }
                            }
                            TSMMain.GUI.GroupManage_List.ScrollIntoView(list[0]);
                            TSMMain.GUI.GroupManage_List.SelectedItem = list[0];
                        }
                        else
                        {
                            var list_2 = ((List<GroupData>)TSMMain.GUI.GroupManage_AllPermission.ItemsSource).Where(p => p.Name.ToLower().Contains(text.ToLower())).ToList();
                            if (list_2.Any())
                            {
                                for (int i = 0; i < list_2.Count; i++)
                                {
                                    if ((GroupData)TSMMain.GUI.GroupManage_AllPermission.SelectedItem == list_2[i])
                                    {
                                        if (list_2.Count > i + 1)
                                        {
                                            TSMMain.GUI.GroupManage_AllPermission.ScrollIntoView(list_2[i + 1]);
                                            TSMMain.GUI.GroupManage_AllPermission.SelectedItem = list_2[i + 1];
                                        }
                                        else
                                        {
                                            TSMMain.GUI.GroupManage_AllPermission.ScrollIntoView(list_2[0]);
                                            TSMMain.GUI.GroupManage_AllPermission.SelectedItem = list_2[0];
                                        }
                                        return;
                                    }
                                }
                                TSMMain.GUI.GroupManage_AllPermission.ScrollIntoView(list_2[0]);
                                TSMMain.GUI.GroupManage_AllPermission.SelectedItem = list_2[0];
                            }
                        }
                    }
                }
                catch { }
            });
        }
    }
}
