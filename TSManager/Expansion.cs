using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ScratchNet;
using Terraria.GameContent.Creative;
using TShockAPI;
using TSManager.Modules;
using TSManager.UI.Control;
using Expression = ScratchNet.Expression;

namespace TSManager
{
    public static class Expansion
    {
        public static void ChangeGodMode(this TSPlayer plr, bool status)
        {
            plr.GodMode = status;
            CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>().SetEnabledState(plr.Index, plr.GodMode);
        }
        public static bool TryGetPlayerInfo(this TSPlayer plr, out Data.PlayerInfo info)
        {
            info = Info.Players.SingleOrDefault(s => s.Name == plr.Name);
            if (info == null) return false;
            return true;
        }
        public static void Add(this RichTextBox t, string content, bool playerInfo, Color color = default)
        {
            TSMMain.GUIInvoke(delegate
            {
                if (color == default) color = Color.FromRgb(245, 245, 245);
                if (t.Document.Blocks.Count <= 0)
                {
                    t.Document.Blocks.Add(new Paragraph());
                }
                Run run = new(content);
                run.Foreground = color.ToBrush();
                run.FontFamily = new FontFamily("Consolas");
                if (playerInfo)
                {
                    Hyperlink hl = new(run);
                    hl.Foreground = color.ToBrush();
                    hl.MouseLeftButtonDown += PlayerClickEvent;
                    hl.Cursor = Cursors.Hand;
                    (t.Document.Blocks.LastBlock as Paragraph).Inlines.Add(hl);
                }
                else (t.Document.Blocks.LastBlock as Paragraph).Inlines.Add(run);
                t.ScrollToEnd();
            });
        }
        static void PlayerClickEvent(object sender, RoutedEventArgs args)
        {
            Hyperlink link = sender as Hyperlink;
            string name = (link.Inlines.FirstInline as Run).Text;
            if (Utils.TryGetPlayerInfo(name, out var plr))
            {
                TSMMain.GUI.MainTab.SelectedIndex = 2;
                TSMMain.GUI.Tab_PlayerManage.SelectedIndex = 0;
                TSMMain.GUI.PlayerManage_List.SelectedItem = plr;
            }
        }
        public static void Add(this RichTextBox t, string content, Color color = default) => Add(t, content, false, color);
        public static void AddLine(this RichTextBox t, string content, Color color = default) => Add(t, content + "\r\n", false, color);

        public static SolidColorBrush ToBrush(this Color color)
        {
            SolidColorBrush solidColorBrush = new(color);
            solidColorBrush.Freeze();
            return solidColorBrush;
        }
        public static Color ToColor(this ConsoleColor color)
        {
            int[] cColors = {   0x000000, //Black = 0
                        0x000080, //DarkBlue = 1
                        0x008000, //DarkGreen = 2
                        0x008080, //DarkCyan = 3
                        0x800000, //DarkRed = 4
                        0x800080, //DarkMagenta = 5
                        0x808000, //DarkYellow = 6
                        0xC0C0C0, //Gray = 7
                        0x808080, //DarkGray = 8
                        0x0000FF, //Blue = 9
                        0x00FF00, //Green = 10
                        0x00FFFF, //Cyan = 11
                        0xFF6060, //Red = 12
                        0xFF00FF, //Magenta = 13
                        0xFFFF00, //Yellow = 14
                        0xFFFFFF  //White = 15
                    };
            if ((int)color == 7) color = ConsoleColor.White;
            var tempcolor = System.Drawing.Color.FromArgb(cColors[(int)color]);
            return Color.FromRgb(tempcolor.R, tempcolor.G, tempcolor.B);
        }

        public static string[] Split(this string text, string splittext) => text.Split(new string[] { splittext }, StringSplitOptions.None);
        public static string[] Split(this string text, string splittext, int count) => text.Split(new string[] { splittext }, count, StringSplitOptions.None);
        public static int ToInt(this string text) => int.TryParse(text, out int i) ? i : -1;
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) return;
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            foreach (T obj in source)
            {
                action(obj);
            }
        }
        public static bool Exist<T>(this IEnumerable<T> source, Func<T, bool> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            if (source.Count() == 0) return false;
            foreach (T obj in source)
            {
                if (action(obj)) return true;
            }
            return false;
        }
        public static void CallOnClick(this ButtonTextBox b)
        {
            //建立一个类型
            Type t = typeof(ButtonTextBox);
            //产生方法
            MethodInfo m = t.GetMethod("Button_Click", BindingFlags.NonPublic | BindingFlags.Instance);
            //参数赋值。传入函数
            //调用
            m.Invoke(b, new object[] { b, null });
            return;
        }
        public static bool TryExcute<T>(this Expression exp, ExecutionEnvironment environment, out T value)
        {
            value = default;
            if (exp is null) return false;
            var result = exp.Execute(environment);
            if (result.Type == CompletionType.Value)
            {
                value = (T)result.ReturnValue;
                return true;
            }
            else if (result.Type == CompletionType.Exception) ScriptManager.Log($"[Warn] 脚本运行时发生错误, 位于 {exp.Type} - {result.ReturnValue}");
            return false;
        }
    }
}
