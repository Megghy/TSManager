using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using TShockAPI;
using TShockAPI.DB;
using TSManager.Modules;

namespace TSManager.Data
{
    [AddINotifyPropertyChangedInterface]
    public class PlayerInfo
    {
        public override string ToString()
        {
            return Name ?? "Unknown";
        }
        public PlayerInfo(TSPlayer player) : this(player.Name, (int)(player.Account?.ID), player.PlayerData, player.Account) { }
        public PlayerInfo(string name, int id, PlayerData data, UserAccount account)
        {
            Name = name;
            ID = id;
            Data = data;
            Account = account;
        }
        public PlayerInfo(UserAccount account) : this(account.Name, account.ID, new(new(-1)), account) { }
        public static List<PlayerInfo> GetAllPlayerInfo()
        {
            var list = new List<PlayerInfo>();
            TShock.UserAccounts.GetUserAccounts().ForEach(a => list.Add(new PlayerInfo(a.Name, a.ID, TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), a.ID), a)));
            return list;
        }
        public void Update()
        {
            try
            {
                Ban = TShock.Bans.Bans.Any(b => b.Value.Identifier == "uuid:" + Account?.UUID) || TShock.Bans.Bans.Any(b => b.Value.Identifier == "acc:" + Name) || (Online && TShock.Bans.Bans.Any(b => b.Value.Identifier == "ip:" + Player.IP));
                if (Online)
                {
                    Data = Player.PlayerData;
                    Account = Player.Account;
                    PlayTime += TSMMain.UpdateTime;
                }
            }
            catch (Exception ex) { ex.ShowError(); }
        }
        public void Save()
        {
            Task.Run(() =>
            {
                if (Data != null)
                {
                    Utils.SaveOffLinePlayerData(Data, ID);
                }
            });
        }
        private bool _Online = false;
        public bool Online
        {
            get => _Online;
            set
            {
                if (_Online != value)
                    PlayTime = 0;
                _Online = value;
            }
        }
        [AlsoNotifyFor("Online", new string[] { "HP", "MaxHP", "MP", "MaxMP", "Ban", "_Ban", "Mute", "GodMode", "KnownIP", "LastLoginTime", "RegisterTime", "Status", "StatusColor" })]
        public long PlayTime { get; set; }
        public string PlayTime_Text { get { TimeSpan ts = new(0, 0, (int)(PlayTime / 1000)); return $"{ts.Days}日 {ts.Hours}时 {ts.Minutes}分 {ts.Seconds}秒"; } set { } }
        public string Status { get => Online ? "在线" : "离线"; set { } }
        public Brush StatusColor { get => Online ? Color.FromRgb(178, 223, 120).ToBrush() : Color.FromRgb(253, 86, 86).ToBrush(); set { } }
        public TSPlayer Player { get; set; } = new TSPlayer(-1);
        public PlayerData Data { get; set; }
        public UserAccount Account { get; set; }
        public string Name { get; set; }
        public int ID { get; set; }
        public int HP
        {
            get => (int)(Online ? Player.TPlayer?.statLife : Data.health);
            set { }
        }
        public int MaxHP
        {
            get => (int)(Online ? Player.TPlayer?.statLifeMax2 : Data.maxHealth);
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
        public int MP { get { return (int)(Online ? Player.TPlayer?.statMana : Data.mana); } set { } }
        public int MaxMP
        {
            get => (int)(Online ? Player.TPlayer?.statManaMax2 : Data.maxMana);
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
        public bool Ban { get; set; }
        public bool _Ban { get => !Ban; set { } }
        public Group Group
        {
            get => Online ? Player.Group : TShock.Groups.GetGroupByName(Account.Group);
            set => TShock.UserAccounts.SetUserGroup(Account, value.Name);
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
            get => Online && Player.mute;
            set
            {
                if (Online)
                {
                    Player.mute = true;
                }
            }
        }
        public bool GodMode
        {
            get => Online && Player.GodMode;
            set
            {
                if (Online)
                {
                    Player.ChangeGodMode(value);
                }
            }
        }
        public string RegisterTime
        {
            get
            {
                try { return Account == null ? "未知" : DateTime.Parse(Account.Registered?.Replace("T", " ")).ToString("F"); }
                catch { return "未知"; }
            }
            set { }
        }
        public string LastLoginTime
        {
            get
            {
                try { return Account == null ? "未知" : DateTime.Parse(Account.LastAccessed?.Replace("T", " ")).ToString("F"); }
                catch { return "未知"; }
            }
            set { }
        }
        public string KnownIP
        {
            get => Account == null ? "未知" : Account.KnownIps;
            set { }
        }
    }
}
