using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using AutoUpdaterDotNET;
using HandyControl.Controls;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using Ionic.Zip;
using Newtonsoft.Json.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TSManager.Data;

namespace TSManager.Modules
{
    public class TSMMain : TerrariaPlugin
    {
        public TSMMain(Main game) : base(game) { }
        public override void Initialize() { }
        public static TSMMain Instance = new TSMMain(null);
        public static TSMWindow GUI { get; internal set; }
        internal static Properties.Settings Settings => Properties.Settings.Default;
        internal static void GUIInvoke(Action action)
        {
            try { GUI.Dispatcher.Invoke(action); }
            catch (Exception ex) { Utils.Notice(ex, HandyControl.Data.InfoType.Error); }
        }
        internal const int UpdateTime = 333;
        public static void AddText(object text, Color color = default)
        {
            color = default ? Color.FromRgb(255, 255, 255) : color;
            Info.Server.OnGetText(text.ToString(), color);
        }
        public static void AddLine(object text, Color color = default) => AddText(text + "\r\n", color);
        public void Stop()
        {
            Task.Run(() =>
            {
                TShock.Utils.StopServer();
            });
        }
        internal void Exit()
        {
            Settings.Save();
            GUIInvoke(() => GUI.Visibility = Visibility.Hidden);
            Info.Server.Stop();
            if (GUI.restart) Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Environment.Exit(0);
        }
        public void Update()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        //更新服务器状态
                        GUIInvoke(() =>
                        {
                            if (Info.IsEnterWorld)
                            {
                                ((ServerStatus)GUI.Tab_Index.DataContext).RunTime += UpdateTime;
                                Info.Players?.ForEach(p => p.Update());
                                GUI.PlayerManage_Count.Content = Info.Players.Count;
                            }
                        });
                        Task.Delay(UpdateTime).Wait();
                    }
                    catch (Exception ex) { Utils.Notice(ex, HandyControl.Data.InfoType.Error); }
                }
            });
        }
        bool firstCheckUpdate = true;
        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.IsUpdateAvailable)
            {
                if (args.Mandatory.Value)
                {
                    try
                    {
                        GUIInvoke(() =>
                        {
                            GUI.TSMVersion_Badge.ShowBadge = true;
                            GUI.TSMVersion_Badge.Text = args.CurrentVersion;
                            GUI.TSMVersion.Text = "当前版本: 发现新版本";
                        });
                        if (firstCheckUpdate)
                        {
                            if (Info.IsEnterWorld) Growl.Ask("发现新版本, 是否现在更新?", result =>
                            {
                                if (result) AutoUpdater.ShowUpdateForm(args);
                                return true;
                            });
                            else AutoUpdater.ShowUpdateForm(args);
                        }
                        firstCheckUpdate = false;
                        /*if (AutoUpdater.DownloadUpdate(args))
                        {
                            Environment.Exit(0);
                        }*/
                    }
                    catch (Exception exception)
                    {
                        Utils.Notice("无法下载更新\r\n" + exception.Message, HandyControl.Data.InfoType.Error);
                    }
                }
            }
        }
        internal void OnInitialize()
        {
            try
            {
                Info.Server.ProcessText(); //循环处理消息队列
                GUI.ChangeNightMode(Settings.EnableDarkMode); //调整暗色模式
                #region 自动更新处理
                AutoUpdater.Start("https://oss.suki.club/TSManager/Update.xml");
                Timer t = new()
                {
                    Interval = 60000,
                    AutoReset = true
                };
                t.Elapsed += (object o, ElapsedEventArgs args) => AutoUpdater.Start("https://oss.suki.club/TSManager/Update.xml");
                t.Start();
                AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
                AutoUpdater.ApplicationExitEvent += delegate
                {
                    if (Info.IsEnterWorld) Info.Server.AppendText("save");
                    else Environment.Exit(0);
                };
                #endregion
                #region 玩家管理器加载代码
                GUI.ServerStatus.Visibility = Visibility.Hidden; //暂时隐藏服务器状态
                GUI.Versions.Visibility = Visibility.Hidden; //暂时隐藏服务器版本
                Info.TextureZip = ZipFile.Read(new MemoryStream(Properties.Resources.Texture)); //加载贴图
                //加载物品前缀
                Dictionary<string, int> prefix = new Dictionary<string, int>();
                var tempjobj = JObject.Parse(Properties.Resources.Prefix);
                foreach (var jobj in tempjobj)
                {
                    int prefixid = jobj.Key.Split("prefix_")[1].ToInt();
                    prefix.Add(prefixid + " - " + (string)jobj.Value, prefixid);
                }
                GUI.Bag_Prefix.ItemsSource = prefix;

                //加载背包界面
                BagManager.CreateBox();
                //添加背包界面选项
                Dictionary<string, int> bags = new()
                {
                    { "主背包", 0 },
                    { "护甲及饰品", 1 },
                    { "染料", 2 },
                    { "猪猪罐", 3 },
                    { "保险箱", 4 },
                    { "守卫者熔炉", 5 },
                    { "虚空", 6 },
                    { "Buff栏", 8 },
                    { "金币.弹药.被选中", 7 }
                };

                GUI.Bag_Choose.ItemsSource = bags;
                #endregion
                #region 设置编辑器加载部分
                //快速搜索功能
                SearchPanel.Install(GUI.ConfigEditor.TextArea);
                //设置语法规则
                using (XmlTextReader reader = new XmlTextReader(Properties.Resources.ResourceManager.GetString("json.xshd"), XmlNodeType.Document, null))
                {
                    var xshd = HighlightingLoader.LoadXshd(reader);
                    GUI.ConfigEditor.SyntaxHighlighting = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
                }
                GUI.ConfigEditor.TextArea.TextEntering += ConfigEdit.OnTextEntering;
                GUI.ConfigEditor.TextArea.TextEntered += ConfigEdit.OnTextEntered;
                var foldingManager = FoldingManager.Install(GUI.ConfigEditor.TextArea);
                var foldingStrategy = new XmlFoldingStrategy();
                foldingStrategy.UpdateFoldings(foldingManager, GUI.ConfigEditor.Document);
                ConfigEdit.LoadAllConfig();

                #endregion
                #region 读取地图列表
                var maps = Maps.GetAllMaps();
                GUI.Console_MapBox.ItemsSource = maps;
                if (Settings.World != string.Empty)
                {
                    var lastMap = maps.SingleOrDefault(m => m.Path == Settings.World);
                    if (lastMap == null)
                    {
                        Utils.Notice("未找到上次启动时所选地图, 请重新选择", HandyControl.Data.InfoType.Info);
                    }
                    else
                    {
                        GUI.Console_MapBox.SelectedItem = lastMap;
                    }
                }
                #endregion
                #region 加载脚本相关
                ScriptManager.LoadAllBlock(); //加载脚本编辑器
                Info.Scripts = new(ScriptData.GetAllScripts());
                GUI.Script_List.ItemsSource = Info.Scripts;
                GUI.Script_TriggerCondition.ItemsSource = Enum.GetValues(typeof(ScriptData.Triggers)).Cast<ScriptData.Triggers>(); ;
                #endregion

                if (Settings.AutoStart)
                {
                    Info.Server.Start(Info.Server.GetStartArgs());
                }
            }
            catch (Exception ex) { Utils.Notice(ex.Message); }
        }
        internal void OnServerPreInitializing()
        {
            GUIInvoke(delegate
            {
                //TSM.MW.Restart_Button.IsEnabled = true; //可以重启了
                GUI.Console_StopServer.IsEnabled = true; //也可以关闭
                GUI.Console_StartServer.IsEnabled = false; //现在不能开启了
            });
            if ((Info.Configs.SingleOrDefault(c => c.Name == "config.json") is { } c && c.JsonData.Value<int>("RestApiPort") is { } restPort && Utils.IsPortInUse(restPort)) || Utils.IsPortInUse(Settings.Port))
            {
                Utils.Notice($"服务器端口 {Settings.Port} 或REST端口被占用, 请在配置文件修改后重新启动", HandyControl.Data.InfoType.Error);
                GUIInvoke(() => GUI.Console_ConsoleBox.Document.Blocks.Clear());
                Info.Server.Stop();
                return;
            }
            Update();
            Utils.Notice("正在启动服务器...", HandyControl.Data.InfoType.Info);
        }
        internal void OnServerPostInitialize(EventArgs args)
        {
            Task.Run(() =>
            {
                //检测一下开没开ssc
                if (!Main.ServerSideCharacter) Utils.Notice("检测到你并未开启服务器云存档, 玩家管理的部分功能将无法生效.");
                //加载服务器状态信息
                GUIInvoke(() =>
                {
                    GUI.Tab_Manage.IsEnabled = true; //可以打开管理界面了
                    GUI.Tab_Index.DataContext = new ServerStatus();
                    GUI.Versions.Visibility = Visibility.Visible;
                    GUI.ServerStatus.Visibility = Visibility.Visible;

                    Info.OnlinePlayers = new();
                    GUI.Console_PlayerList.ItemsSource = Info.OnlinePlayers; //设置玩家列表绑定数据

                    PlayerManager.Refresh();

                    GUI.GroupManage_AllPermission.ItemsSource = GroupData.GetAllPermissions(); //尝试读取所有权限
                    GroupManager.RefreshGroupData();//读取所有组权限
                });

                // 注册一些用得到的hook
                HookManager.RegisterHooks();

                Utils.Notice("服务器启动完成", HandyControl.Data.InfoType.Success);
                Info.IsEnterWorld = true;
            });
        }
    }
}
