using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TSManager.Data;
using TSManager.Modules;
using HookManager = TSManager.Modules.HookManager;

namespace TSManager
{
    public class TSMMain : TerrariaPlugin
    {
        public TSMMain(Main game) : base(game) { }
        public override string Author => "Megghy";
        public override string Description => "其实压根不需要继承plugin类";
        public override string Name => "TSManager";
        public override Version Version => Environment.Version;
        public override void Initialize() { }
        public static TSMMain Instance = new(null);
        public static TSMWindow GUI { get; internal set; }
        internal static Properties.Settings Settings => Properties.Settings.Default;
        internal static void GUIInvoke(Action action)
        {
            try { GUI.Dispatcher.Invoke(action); }
            catch (Exception ex) { ex.ShowError(); }
        }
        internal const int UpdateTime = 500;
        public void StopServer() => TShock.Utils.StopServer();
        internal void OnExit()
        {
            Info.Server.Stop();
            Settings.Save();
            GUIInvoke(() => GUI.Visibility = Visibility.Hidden);
            if (GUI.reStart)
                Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Environment.Exit(0);
        }
        internal void Update()
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
                                Info.OnlinePlayers?.ForEach(p => p.PlayTime += UpdateTime);
                                PlayerManager.SelectedPlayerInfo?.Update();
                                GUI.PlayerManage_Count.Content = "总人数: " + Info.Players.Count;
                            }
                        });
                        Task.Delay(UpdateTime).Wait();
                    }
                    catch (Exception ex) { ex.ShowError(); }
                }
            });
        }

        internal void OnInitialize()
        {
            try
            {
                //循环处理消息队列
                GUI.ServerStatus.Visibility = Visibility.Hidden; //暂时隐藏服务器状态
                GUI.Versions.Visibility = Visibility.Hidden; //暂时隐藏服务器版本               
                GUI.ChangeNightMode(Settings.EnableDarkMode ? HandyControl.Data.SkinType.Dark : HandyControl.Data.SkinType.Default); //调整暗色模式

                UI.GUIEvents.RegisterAll(); //加载所有用户界面处理代码

                #region 加载tsapi程序集
                Info.Server = new(typeof(ServerApi).Assembly);
                ServerManger.Init();
                #endregion

                #region 自动更新处理
                Utils.DisplayInfo("正在检查更新...");
                Updater.Init();
                #endregion

                #region 背包管理器加载代码
                Utils.DisplayInfo("加载背包管理器...");
                BagManager.Init();
                #endregion

                #region 设置编辑器加载部分
                Utils.DisplayInfo("初始化设置编辑器...");
                ConfigEdit.Init();
                #endregion

                #region 读取地图列表
                Utils.DisplayInfo("读取地图列表...");
                MapManager.Init();
                #endregion

                #region 加载脚本相关
                Utils.DisplayInfo("初始化脚本编辑器...");
                ScriptManager.Init();
                #endregion

                Utils.DisplayInfo("初始化完成. 欢迎使用 TSManager.");

                if (Settings.AutoStart)
                {
                    Info.Server.Start(Info.Server.GetStartArgs());
                }
            }
            catch (Exception ex) { ex.ShowError(); }
        }
        internal void OnServerPreInitializing()
        {
            GUIInvoke(delegate
            {
                GUI.Console_StopServer.IsEnabled = true; //也可以关闭
                GUI.Console_StartServer.IsEnabled = false; //现在不能开启了
                GUI.Console_RestartServer.IsEnabled = false; //暂时不能重启
                GUI.GoToStartServer.IsEnabled = false;
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
            if (TShock.VersionNum < new Version(4, 5, 0))
            {
                Utils.Notice("TSManager 不支持版本低于 4.5.0 的TShock, 即将关闭服务器");
                Task.Delay(2000).Wait();
                StopServer();
                return;
            }
            ServerManger.TSMHarmony.Patch(typeof(TShockAPI.Utils).GetMethod("StopServer"), null, ServerManger.GetMethod("OnServerStop")); //tshock加载完成后可以开始patch了
                                                                                                                                          //检测一下开没开ssc
            if (!Main.ServerSideCharacter)
                Utils.Notice("检测到你并未开启服务器云存档, 玩家管理的部分功能将无法生效.");
            //加载服务器状态信息
            GUIInvoke(() =>
            {
                GUI.Console_RestartServer.IsEnabled = true; //可以重启了
                GUI.Tab_Manage.IsEnabled = true; //可以打开管理界面了
                GUI.Tab_Index.DataContext = new ServerStatus();
                GUI.Versions.Visibility = Visibility.Visible;
                GUI.ServerStatus.Visibility = Visibility.Visible;

                Info.OnlinePlayers = new();
                GUI.Console_PlayerList.ItemsSource = Info.OnlinePlayers; //设置玩家列表绑定数据

                PlayerManager.Refresh();

                GroupManager.Init();
                Utils.DisplayInfo("已读取用户组信息");
            });

            // 注册一些用得到的hook
            HookManager.RegisterHooks();
            Utils.DisplayInfo("成功注册Hooks");

            Utils.Notice("服务器启动完成", HandyControl.Data.InfoType.Success);
            Info.IsEnterWorld = true;
        }
    }
}
