using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using TShockAPI;

namespace TSManager.Data
{
    class GroupData
    {
        public override string ToString()
        {
            return Name;
        }
        public static List<PermissionData> GetAllPermissions()
        {
            var permissions = new List<PermissionData>();
            Commands.ChatCommands.ForEach(c =>
            {
                if (c.Permissions.Count > 1) c.Permissions.ForEach(p => { if (p != "") permissions.Add(new PermissionData(p, "/" + string.Join(",", c.Names))); });
                else if (c.Permissions.Any() && c.Permissions[0] != "") permissions.Add(new PermissionData(c.Permissions[0], "/" + string.Join(",", c.Names)));
            });
            TShock.Groups.ForEach(g => g.permissions.ForEach(p => { if (!permissions.Where(_p => _p.Name == p).Any()) permissions.Add(new PermissionData(p, "未知")); }));
            return permissions;
        }
        public async static Task<List<PermissionData>> GetPermissionsAsync(string groupName)
        {
            var list = new List<PermissionData>();
            await Task.Run(() =>
            {
                var group = TShock.Groups.GetGroupByName(groupName);
                if (group == null) return list;
                else if (group.permissions == null) return list;
                var templist = group.TotalPermissions.ToArray().ToList();
                group.permissions.ForEach(p => templist.Remove(p));
                list.Add(new PermissionData(templist.Any() ? string.Join("\n", templist) : "无", "所继承的权限", false));
                templist.Clear();
                templist = group.permissions.ToArray().ToList();
                group.permissions.ForEach(p => Commands.ChatCommands.Where(c => c.Permissions != null && c.Permissions.Contains(p)).ForEach(c => { if (p != "" && p != " ") list.Add(new PermissionData(p, TShock.Config.Settings.CommandSpecifier + string.Join(",", c.Names))); templist.Remove(p); }));
                templist.ForEach(p => { if (p != "" && p != " ") list.Add(new PermissionData(p, "未知")); });
                return list;
            });
            return list;
        }
        public GroupData(Group group, bool getpermission = false)
        {
            Name = group.Name;
            Group = group;
            Prefix = group.Prefix;
            Suffix = group.Suffix;
            ChatColor = Color.FromRgb(group.R, group.G, group.B);
            if (getpermission) Permissions = new(GetPermissionsAsync(Name).Result);
            if (group.Parent != null) Parent = new GroupData(group.Parent, getpermission);
        }

        public GroupData(bool noParent = false)
        {
            Name = "Unknown";
            Group = new Group("Unknown");
            ChatColor = default;
            NoParent = noParent;
        }
        /// <summary>
        /// 指示是否为combobox中表示为空父组的对象
        /// </summary>
        public bool NoParent;
        public string Name { get; set; }
        public Group Group { get; set; }
        public BindingList<PermissionData> Permissions { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public Color ChatColor { get; set; }
        public Brush DisplayColor
        {
            get { return ChatColor.ToBrush(); }
            set
            {
                if (ChatColor == null) return;
                Color color = ((SolidColorBrush)value).Color;
                ChatColor = Color.FromRgb(color.R, color.G, color.B);
            }
        }
        public GroupData Parent { get; set; }
        public GroupData ChangeParent { set { } }
    }
    public class PermissionData
    {
        public PermissionData(string name, string command, bool candel = true)
        {
            Name = name;
            Command = command;
            CanDel = candel;
        }
        public PermissionData(string name)
        {
            Name = name;
            var cmd = Commands.ChatCommands.Where(c => c.Permissions.Contains(name)).ToList();
            Command = cmd.Any() ? Commands.Specifier + string.Join(", ", cmd) : "未知";
            CanDel = true;
        }
        public bool CanDel { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }
    }
}
