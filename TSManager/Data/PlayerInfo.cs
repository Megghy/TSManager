using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using PropertyChanged;
using Terraria.GameContent;
using TShockAPI;
using TShockAPI.DB;
using TSManager.Modules;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace TSManager.Data
{
    [AddINotifyPropertyChangedInterface]
    public class PlayerInfo
    {
        public override string ToString()
        {
            return Name ?? "Unknown";
        }
        public PlayerInfo(string name, int id, TSPlayer player)
        {
            Name = name;
            ID = id;
            Data = player.PlayerData;
            Account = player.Account;
        }
        public PlayerInfo(string name, int id, PlayerData data, UserAccount account)
        {
            Name = name;
            ID = id;
            Data = data;
            Account = account;
        }
        public PlayerInfo()
        {
            ID = -1;
            Data = new PlayerData(new TSPlayer(-1));
        }
        public static List<PlayerInfo> GetAllPlayerInfo()
        {
            var list = new List<PlayerInfo>();
            TShock.UserAccounts.GetUserAccounts().ForEach(a => list.Add(new PlayerInfo(a.Name, a.ID, TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), a.ID), a)));
            return list;
        }
        public void Update()
        {
            Player = TShock.Players.SingleOrDefault(p => p != null && p.Name == Name);
            if (Player == null)
            {
                if (Online) Online = false;
            }
            else
            {               
                Data = Player.PlayerData;
                Account = Player.Account;
                PlayTime += TSMMain.UpdateTime;
                Online = true;
            }
        }
        public async void Save()
        {
            await Task.Run(() =>
            {
                if (Data != null)
                {
                    Utils.SaveOffLinePlayerData(Data, ID);
                }
            });
        }
        public bool Online { get; set; }
        [AlsoNotifyFor("Online", new string[] { "HP", "MaxHP", "MP", "MaxMP", "Mute", "GodMode", "KnownIP", "LastLoginTime", "RegisterTime", "Status", "StatusColor" })]
        public long PlayTime { get; set; }
        public string Status { get { return Online ? "在线" : "离线"; } set { } }
        public Brush StatusColor { get { return Online ? Color.FromRgb(178, 223, 120).ToBrush() : Color.FromRgb(253, 86, 86).ToBrush(); } set { } }
        public TSPlayer Player { get; set; } = new TSPlayer(-1);
        public PlayerData Data { get; set; } = new PlayerData(null);
        public UserAccount Account { get; set; } = new UserAccount();
        public string Name { get; set; }
        public int ID { get; set; }
        public int HP
        {
            get
            {
                return Online ? Player.TPlayer.statLife: Data.health;
            }
            set { }
        }
        public int MaxHP
        {
            get
            {
                return Online ? Player.TPlayer.statLifeMax2 : Data.maxHealth;
            }
            set
            {
                if (Online)
                {
                    Player.TPlayer.statLifeMax = value;
                    Player.SendData(PacketTypes.PlayerInfo);
                }
                else
                {
                    Data.maxHealth = value;
                    Save();
                }
            }
        }
        public int MP { get { return Online ? Player.TPlayer.statMana : Data.mana; } set { } }
        public int MaxMP
        {
            get
            {
                return Online ? Player.TPlayer.statManaMax2 : Data.maxMana;
            }
            set
            {
                if (Online)
                {
                    Player.TPlayer.statManaMax = value;
                    Player.SendData(PacketTypes.PlayerInfo);
                }
                else
                {
                    Data.maxMana = value;
                    Save();
                }
            }
        }
        public Group Group
        {
            get
            {
                if (Online) return Player.Group;
                else return TShock.Groups.GetGroupByName(Account.Group);

            }
            set
            {
                TShock.UserAccounts.SetUserGroup(Account, value.Name);
            }
        }
        public List<Group> Groups
        {
            get
            {
                return TShock.Groups.groups;
            }
            set
            {
            }
        }
        public bool Mute
        {
            get { return Online ? Player.mute : false; }
            set
            {
                if (Online)
                {
                    Player.mute = true;
                    Player.SendInfoMessage($"你已被{(value ? "" : "取消")}禁言.");
                    Utils.Notice($"{Name} 已{(value ? "" : "取消")}禁言", HandyControl.Data.InfoType.Success);
                }
            }
        }
        public bool GodMode
        {
            get { return Online ? Player.GodMode : false; }
            set
            {
                if (Online)
                {
                    Player.ChangeGodMode(value);
                    Player.SendInfoMessage($"你已{(value ? "开启" : "关闭")}上帝模式.");
                    Utils.Notice($"{Name} 已{(value ? "开启" : "关闭")}上帝模式", HandyControl.Data.InfoType.Success);
                }
            }
        }
        public string RegisterTime
        {
            get
            {
                try { return DateTime.Parse((Account.Registered ?? "未知").Replace("T", " ")).ToString("F"); }
                catch { return "未知"; }
            }
            set { }
        }
        public string LastLoginTime
        {
            get
            {
                try { return DateTime.Parse((Account.LastAccessed ?? "未知").Replace("T", " ")).ToString("F"); }
                catch { return "未知"; }
            }
            set { }
        }
        public string KnownIP
        {
            get
            {
                return Account.KnownIps;
            }
            set { }
        }
    }
}
