using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HandyControl.Controls;
using HandyControl.Data;
using Ionic.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TShockAPI;
using TShockAPI.DB;
using TSManager.Data;
using TSManager.Modules;

namespace TSManager
{
    internal static class Utils
    {
        public static bool SaveOffLinePlayerData(PlayerData data, int id)
        {
            if (id == -1) return false;
            if (TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), id) == null)
            {
                try
                {
                    TShock.CharacterDB.database.Query("INSERT INTO tsCharacter (Account, Health, MaxHealth, Mana, MaxMana, Inventory, extraSlot, spawnX, spawnY, skinVariant, hair, hairDye, hairColor, pantsColor, shirtColor, underShirtColor, shoeColor, hideVisuals, skinColor, eyeColor, questsCompleted) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15, @16, @17, @18, @19, @20);", new object[]
                    {
                        id,
                        data.health,
                        data.maxHealth,
                        data.mana,
                        data.maxMana,
                        string.Join<NetItem>("~", data.inventory),
                        data.extraSlot,
                        data.spawnX,
                        data.spawnX,
                        data.skinVariant,
                        data.hair,
                        data.hairDye,
                        TShock.Utils.EncodeColor(data.hairColor),
                        TShock.Utils.EncodeColor(data.pantsColor),
                        TShock.Utils.EncodeColor(data.shirtColor),
                        TShock.Utils.EncodeColor(data.underShirtColor),
                        TShock.Utils.EncodeColor(data.shoeColor),
                        TShock.Utils.EncodeBoolArray(data.hideVisuals),
                        TShock.Utils.EncodeColor(data.skinColor),
                        TShock.Utils.EncodeColor(data.eyeColor),
                        data.questsCompleted
                    });
                    return true;
                }
                catch (Exception ex)
                {
                    ex.ShowError();
                    return false;
                }
            }
            try
            {
                TShock.CharacterDB.database.Query("UPDATE tsCharacter SET Health = @0, MaxHealth = @1, Mana = @2, MaxMana = @3, Inventory = @4, spawnX = @6, spawnY = @7, hair = @8, hairDye = @9, hairColor = @10, pantsColor = @11, shirtColor = @12, underShirtColor = @13, shoeColor = @14, hideVisuals = @15, skinColor = @16, eyeColor = @17, questsCompleted = @18, skinVariant = @19, extraSlot = @20 WHERE Account = @5;", new object[]
                {
                    data.health,
                    data.maxHealth,
                    data.mana,
                    data.maxMana,
                    string.Join<NetItem>("~", data.inventory),
                    id,
                    data.spawnX,
                    data.spawnX,
                    data.skinVariant,
                    data.hair,
                    data.hairDye,
                    TShock.Utils.EncodeColor(data.hairColor),
                    TShock.Utils.EncodeColor(data.pantsColor),
                    TShock.Utils.EncodeColor(data.shirtColor),
                    TShock.Utils.EncodeColor(data.underShirtColor),
                    TShock.Utils.EncodeColor(data.shoeColor),
                    TShock.Utils.EncodeBoolArray(data.hideVisuals),
                    TShock.Utils.EncodeColor(data.skinColor),
                    TShock.Utils.EncodeColor(data.eyeColor),
                    data.questsCompleted,
                    data.extraSlot ?? 0
                });
                return true;
            }
            catch (Exception ex2)
            {
                TShock.Log.Error(ex2.ToString());
            }
            return false;
        }
        public static bool TryGetPlayerInfo(string name, out PlayerInfo info)
        {
            info = Info.Players.SingleOrDefault(p => p.Name == name);
            if (info == null) return false;
            return true;
        }
        public static void Notice(object text, InfoType type = InfoType.Info, int delayTime = 4)
        {
            switch (type)
            {
                case InfoType.Error:
                    Growl.Error(new GrowlInfo() { Message = text.ToString(), Type = type, WaitTime = delayTime });
                    break;
                case InfoType.Success:
                    Growl.Success(new GrowlInfo() { Message = text.ToString(), Type = type, WaitTime = delayTime });
                    break;
                case InfoType.Ask:
                    Growl.Ask(new GrowlInfo() { Message = text.ToString(), Type = type, WaitTime = delayTime });
                    break;
                case InfoType.Warning:
                    Growl.Warning(new GrowlInfo() { Message = text.ToString(), Type = type, WaitTime = delayTime });
                    break;
                case InfoType.Fatal:
                    Growl.Fatal(new GrowlInfo() { Message = text.ToString(), Type = type, WaitTime = delayTime });
                    break;
                default:
                    Growl.Info(new GrowlInfo() { Message = text.ToString(), Type = type, WaitTime = delayTime });
                    break;
            }
        }
        public static void DisplayInfo(object text, string prefix = "TSManager")
        {
            AddText("[");
            AddText(prefix, Color.FromRgb(145, 190, 215));
            AddText("] ");
            AddLine(text, Color.FromRgb(230, 230, 170));
        }
        public static void ShowError(this Exception ex, InfoType type = InfoType.Error)
        {
            Notice($"{new StackTrace(1)}\r\n{ex.Message}", type);
        }
        public static void AddText(object text, Color color = default)
        {
            color = color == default ? Color.FromRgb(255, 255, 255) : color;
            Info.Server.DisplayText(text?.ToString(), color);
        }
        public static void AddLine(object text = null, Color color = default) => AddText(text + "\r\n", color);
        public static bool TryParseJson(string json, out JObject jobj)
        {
            try { jobj = JObject.Parse(json); return true; } catch { jobj = null; ; return false; }
        }
        public static string ConvertJsonString(string str)
        {
            //格式化json字符串
            JsonSerializer serializer = new();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new();
                JsonTextWriter jsonWriter = new(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }
        public static bool IsPortInUse(int port)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    return true;
                }
            }
            return false;
        }
        public async static Task<BitmapImage> GetTexture(string filename)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (Stream tempstream = new MemoryStream())
                    {
                        ZipEntry entry = Info.TextureZip[filename];
                        entry.Extract(tempstream);
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad; // here
                        image.StreamSource = tempstream;
                        image.EndInit();
                        image.Freeze();
                        return image;
                    }
                }
                catch { return null; }
            });
        }
    }
}
