using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using HarmonyLib;
using OTAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace TSManager.Modules
{
    public sealed partial class ServerManger : IDisposable
    {
        private static ServerManger Instance => Info.Server;
        private readonly Assembly mainasm;
        private readonly BlockingStream istream;
        //private readonly HashSet<Thread> ThreadList = new HashSet<Thread>();
        private static MethodInfo GetMethod(string name)
        {
            return typeof(ServerManger).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);
        }
        private Color foregroundColor = ConsoleColor.White.ToColor();

        private static bool OnSetForegroundColor(ConsoleColor value)
        {
            Console.Out.Flush();
            Instance.foregroundColor = value.ToColor();
            return false;
        }

        private static bool OnResetColor()
        {
            Console.Out.Flush();
            Instance.foregroundColor = ConsoleColor.White.ToColor();
            return false;
        }
        public ServerManger(Assembly server)
        {
            mainasm = server;
            var console = typeof(Console);
            var harmony = new Harmony("servermgr" + GetHashCode());

            istream = new BlockingStream();
            Console.SetIn(new StreamReader(istream, Encoding.UTF8));
            Console.SetOut(new ConsoltOut());
            harmony.Patch(console.GetProperty("Title").SetMethod, new HarmonyMethod(GetMethod("OnSetTitle")));
            harmony.Patch(console.GetMethod("ResetColor"), new HarmonyMethod(GetMethod("OnResetColor")));
            harmony.Patch(console.GetProperty("ForegroundColor").SetMethod, new HarmonyMethod(GetMethod("OnSetForegroundColor")));
            harmony.Patch(console.GetMethod("Clear"), new HarmonyMethod(GetMethod("ClearText")));
            var _ = Console.IsInputRedirected;
        }
        private static void ClearText()
        {
            TSMMain.GUIInvoke(() => TSMMain.GUI.Console_ConsoleBox.Document.Blocks.Clear());
        }
        private static bool OnSetTitle(string value)
        {
            TSMMain.GUIInvoke(() => TSMMain.GUI.Console_MainGroup.Header = $"{value}");
            return false;
        }
        class TextInfo
        {
            public TextInfo(string text, bool isPlayer = false)
            {
                IsPlayer = isPlayer;
                Text = text;
            }
            public bool IsPlayer { get; set; }
            public string Text { get; set; }
            public override string ToString()
            {
                return Text;
            }
        }
        Color PlayerColor = Color.FromRgb(Microsoft.Xna.Framework.Color.Aquamarine.R, Microsoft.Xna.Framework.Color.Aquamarine.G, Microsoft.Xna.Framework.Color.Aquamarine.B);
        public void OnGetText(string s)
        {
            if (s == " \r\n")
            {
                TSMMain.GUI.Console_ConsoleBox.AddLine(" ");
                return;
            }
            if (Info.IsEnterWorld)
            {
                var nameList = Info.Players.Select(p => p.Name).ToList();
                var list = new List<TextInfo>() { new TextInfo(s, false) };
                if (nameList.Any())
                {
                    nameList.ForEach(name =>
                    {
                        var tempList = new List<TextInfo>();
                        list.ForEach(t =>
                        {
                            if (!t.IsPlayer && t.Text.Contains(name))
                            {
                                var splitText = t.Text.Split(name);
                                tempList.Add(new TextInfo(splitText[0], false));
                                tempList.Add(new TextInfo(name, true));
                                tempList.Add(new TextInfo(splitText[1], false));
                            }
                            else
                            {
                                tempList.Add(t);
                            }
                        });
                        list = tempList;
                    });
                    list.ForEach(t => TSMMain.GUI.Console_ConsoleBox.Add(t.Text, t.IsPlayer, t.IsPlayer ? PlayerColor : foregroundColor));
                    return;
                }
            }
            TSMMain.GUI.Console_ConsoleBox.Add(s, foregroundColor);
        }
        public string[] GetStartArgs()
        {
            return new List<string>
            {
                $"-port",
                $"{TSMMain.Settings.Port}",
                $"-password",
                $"{TSMMain.Settings.Password}",
                $"-language",
                $"{(TSMMain.Settings.EnableChinese ? "zh-Hans" : " ")}",
                $"-world",
                //$"{(WorldCombo.SelectedIndex == -1 ? " " : ($"{WorldCombo.SelectedValue}"))}",
                $"{(File.Exists(TSMMain.Settings.World) ? TSMMain.Settings.World : "")}",
                $"-players",
                $"{TSMMain.Settings.MaxPlayer}"
            }.ToArray();
        }
        public void Stop()
        {
            Utils.Notice("正在关闭...");
            Info.IsServerRunning = false;
            Info.GameThread.Abort();
        }

        public void Dispose()
        {
            Stop();
        }

        public void Start(string[] param)
        {
            if (!Directory.Exists(Info.PluginPath) || !Directory.GetFiles(Info.PluginPath).Exist(f => Path.GetFileName(f) == "TShockAPI.dll"))
            {
                Utils.Notice($"未检测到TShock程序集. TSManager暂不支持原版服务器.", HandyControl.Data.InfoType.Error);
                TSMMain.GUIInvoke(() => {
                    TSMMain.GUI.Console_StartServer.IsEnabled = true;
                });
                Info.IsServerRunning = false;
                return;
            }
            Info.GameThread = new Thread(new ThreadStart(() =>
            {
                mainasm.EntryPoint.Invoke(null, new object[] { param });
            }))
            {
                IsBackground = true
            };
            Info.IsServerRunning = true;

            Info.GameThread.Start();
            new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    if (Netplay.Disconnect)
                    {
                        TSMMain.Instance.Exit();
                        return;
                    }
                    Task.Delay(50).Wait();
                }
            }))
            {
                IsBackground = true
            }.Start();  //似乎只能这样判断服务器是否关闭
            ServerApi.Hooks.GamePostInitialize.Register(TSMMain.Instance, TSMMain.Instance.OnServerPostInitialize, -1);  //注册服务器加载完成的钩子
            TSMMain.Instance.OnServerPreInitializing();
            TSMMain.GUIInvoke(() => TSMMain.GUI.GoToStartServer.IsEnabled = false);
        }

        public async void AppendText(string text)
        {
            await Task.Run(() => {
                TSMMain.GUI.Console_ConsoleBox.AddLine(text, Color.FromRgb(208, 171, 233));
                if (Info.IsEnterWorld)
                {
                    Commands.HandleCommand(TSPlayer.Server, TShock.Config.Settings.CommandSpecifier + text);
                }
                else
                {
                    istream.AppendText(text == "" || text == null ? " " : text);
                    istream.AppendText("");
                }
            });
        }
    }
    class ConsoltOut : TextWriter
    {
        public ConsoltOut()
        {
        }
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
        public override void Write(string value)
        {
            Info.Server.OnGetText(value);
        }
        public override void WriteLine(string value)
        {
            Info.Server.OnGetText(value + "\r\n");
        }
        public override void WriteLine()
        {
            Info.Server.OnGetText(" \r\n");
        }
    }
    public sealed partial class ServerManger
    {
        private class BlockingStream : Stream
        {
            private byte[] lasting = new byte[0];
            private BlockingCollection<byte[]> queue = new BlockingCollection<byte[]>();

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => 0;

            public override long Position { get; set; }

            public override void Flush() { }

            public void AppendText(string text)
            {
                queue.Add(Encoding.UTF8.GetBytes(text));
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (lasting.Length == 0)
                {
                    Console.Out.Flush();
                    lasting = queue.Take();
                }

                var n = lasting.Length;
                if (n > count)
                {
                    Buffer.BlockCopy(lasting, 0, buffer, 0, count);
                    lasting = lasting.Skip(count).ToArray();
                    return count;
                }
                else
                {
                    Buffer.BlockCopy(lasting, 0, buffer, 0, n);
                    lasting = new byte[0];
                    return n;
                }
            }

            public override long Seek(long offset, SeekOrigin origin) => 0;

            public override void SetLength(long value) { }

            public override void Write(byte[] buffer, int offset, int count) { }
        }
    }
}
