using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using Ionic.Zip;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.GameContent;
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
        public static readonly TSMWindow GUI = new TSMWindow();
        internal static Properties.Settings Settings => Properties.Settings.Default;
        internal static void GUIInvoke(Action action)
        {
            try { GUI.Dispatcher.Invoke(action); }
            catch (Exception ex) { Utils.Notice(ex, HandyControl.Data.InfoType.Error); }
        }
        internal const int UpdateTime = 333;
        public async void Stop()
        {
            await Task.Run(() =>
            {
                TShock.Utils.StopServer();
            });
        }
        internal void Exit()
        {
            GUIInvoke(() => GUI.Visibility = Visibility.Hidden);
            Info.Server.Stop();
            if (GUI.restart) Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Environment.Exit(0);
        }
        public async void Update()
        {
            await Task.Run(() =>
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
                                Info.Players.ForEach(p => p.Update());
                            }
                        });
                        Task.Delay(UpdateTime).Wait();
                    }
                    catch (Exception ex) { Utils.Notice(ex, HandyControl.Data.InfoType.Error); }
                }
            });
        }
        internal void OnInitialize()
        {
            try
            {
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

                Info.Configs = ConfigData.ReadAllConfig();
                GUI.ConfigEditor_List.ItemsSource = Info.Configs;
                GUI.ConfigEditor_List.SelectedItem = Info.Configs.FirstOrDefault(c => c.Name == "config.json");
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

            }
            catch (Exception ex) { Utils.Notice(ex.Message); }
            Instance.Update();
        }
        internal void OnServerPreInitializing()
        {
            GUIInvoke(delegate
            {
                //TSM.MW.Restart_Button.IsEnabled = true; //可以重启了
                GUI.Console_StopServer.IsEnabled = true; //也可以关闭
                GUI.Console_StartServer.IsEnabled = false; //现在不能开启了
                GUI.Tab_Manage.IsEnabled = true; //可以打开管理界面了

            });
            Utils.Notice("正在启动服务器...", HandyControl.Data.InfoType.Info);
        }
        internal async void OnServerPostInitialize(EventArgs args)
        {
            await Task.Run(() =>
            {

                //加载服务器状态信息
                GUIInvoke(() =>
                {
                    GUI.Tab_Index.DataContext = new ServerStatus();
                    GUI.Versions.Visibility = Visibility.Visible;
                    GUI.ServerStatus.Visibility = Visibility.Visible;

                    Info.Players = PlayerInfo.GetAllPlayerInfo();
                    GUI.PlayerManage_List.ItemsSource = Info.Players;
                    if (Info.Players.Any()) GUI.PlayerManage_List.SelectedItem = Info.Players[0];
                });


                Utils.Notice("服务器启动完成", HandyControl.Data.InfoType.Success);
                Info.IsEnterWorld = true;
            });
        }
    }
}
