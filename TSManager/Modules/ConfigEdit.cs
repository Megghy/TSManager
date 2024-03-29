﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using HandyControl.Controls;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using Newtonsoft.Json.Linq;
using TShockAPI;
using TShockAPI.Hooks;
using TSManager.Data;

namespace TSManager.Modules
{
    internal static class ConfigEdit
    {
        public static ConfigData SelectedConfigData => TSMMain.GUI.ConfigEditor_List.SelectedItem as ConfigData;
        internal static void Init()
        {
            //快速搜索功能
            SearchPanel.Install(TSMMain.GUI.ConfigEditor.TextArea);
            //设置语法规则
            using (XmlTextReader reader = new(Properties.Resources.ResourceManager.GetString("json.xshd"), XmlNodeType.Document, null))
            {
                var xshd = HighlightingLoader.LoadXshd(reader);
                TSMMain.GUI.ConfigEditor.SyntaxHighlighting = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
            }
            TSMMain.GUI.ConfigEditor.TextArea.TextEntering += OnTextEntering;
            TSMMain.GUI.ConfigEditor.TextArea.TextEntered += OnTextEntered;
            var foldingManager = FoldingManager.Install(TSMMain.GUI.ConfigEditor.TextArea);
            var foldingStrategy = new XmlFoldingStrategy();
            foldingStrategy.UpdateFoldings(foldingManager, TSMMain.GUI.ConfigEditor.Document);
            LoadAllConfig();
        } 
        public static void LoadAllConfig()
        {
            Info.Configs = new(ConfigData.ReadAllConfig());
            TSMMain.GUIInvoke(() =>
            {
                TSMMain.GUI.ConfigEditor_List.ItemsSource = Info.Configs;
                TSMMain.GUI.ConfigEditor_List.SelectedItem = Info.Configs.FirstOrDefault(c => c.Name == "config.json");
            });
        }
        public static void OnTextEntering(object sender, TextCompositionEventArgs e)
        {

        }
        public static void OnTextEntered(object sender, TextCompositionEventArgs e)
        {
            var editor = TSMMain.GUI.ConfigEditor;
            switch (e.Text)
            {
                /*case "t":
                    var completionWindow = new CompletionWindow(TSMMain.GUI.ConfigEditor.TextArea);
                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                    data.Add(new Data.ConfigProcess("true"));
                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                    break;
                case "f":
                    completionWindow = new CompletionWindow(TSMMain.GUI.ConfigEditor.TextArea);
                    data = completionWindow.CompletionList.CompletionData;
                    data.Add(new Data.ConfigProcess("false"));
                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                    break;*/
                case "\"":
                    int i = TSMMain.GUI.ConfigEditor.SelectionStart;
                    editor.Focus();
                    editor.Text = editor.Text.Insert(i, "\"");
                    editor.Select(i, 0);
                    break;
                case "{":
                    i = editor.SelectionStart;
                    editor.Focus();
                    editor.Document.Insert(i, "}");
                    editor.Select(i, 0);
                    break;
                case "[":
                    i = editor.SelectionStart;
                    editor.Focus();
                    editor.Document.Insert(i, "]");
                    editor.Select(i, 0);
                    break;
            }
        }
        public static void OnTextChange(Data.ConfigData data)
        {
            Task.Run(() =>
            {
                if (data == null) return;
                try
                {
                    TSMMain.GUIInvoke(() => data.Text = TSMMain.GUI.ConfigEditor.Text);
                    data.Error = !Utils.TryParseJson(data.Text, out var _);
                }
                catch { data.Error = true; }
            });
        }
        public static void Save(ConfigData data, bool force = false)
        {
            try
            {
                if (Utils.TryParseJson(data.Text, out var json)) data.JsonData = json; //首先转换一下文本
                if (force || !data.Error)
                {
                    using (StreamWriter sw = new(data.Path))
                    {
                        sw.WriteLine(data.Text);
                        sw.Flush();
                    }
                    if (Info.IsServerRunning) ReloadCfg();
                    Utils.Notice($"已保存该配置文件.{(Info.IsServerRunning ? "\r\n仅有部分插件支持重载, 一些设置项修改后可能需要重启后才能生效." : "")}", HandyControl.Data.InfoType.Success);
                }
                else Growl.Ask("Json格式存在错误, 确定要保存吗?", b =>
                {
                    if (b) Save(data, true);
                    return true;
                });
            }
            catch (Exception ex)
            {
                Utils.Notice("保存配置文件失败.\r\n" + ex, HandyControl.Data.InfoType.Error);
            }
        }
        /// <summary>
        /// 不加载程序集直接保存会报错
        /// </summary>
        static void ReloadCfg() => GeneralHooks.OnReloadEvent(TSPlayer.Server);
        public static void OpenFile(ConfigData data)
        {
            Process.Start("notepad.exe", data.Path);
        }
        public static void ChangeConfig(ConfigData data)
        {
            if (data == null)
            {
                //Utils.Notice("配置文件无效.", HandyControl.Data.InfoType.Error);
                return;
            }
            data.ErrorCheck();
            TSMMain.GUI.Tab_Editor.DataContext = data;
            TSMMain.GUI.ConfigEditor.Text = data.Text;
        }
        public static void Format(ConfigData data)
        {
            try
            {
                data.JsonData = JObject.Parse(data.Text);
                data.Text = Utils.ConvertJsonString(data.Text);
                TSMMain.GUI.ConfigEditor.Text = data.Text;
                Utils.Notice($"已格式化配置文件.", HandyControl.Data.InfoType.Success);
            }
            catch (Exception ex) { Utils.Notice($"Json格式存在错误.\r\n{ex}", HandyControl.Data.InfoType.Error); }
        }
    }
}
