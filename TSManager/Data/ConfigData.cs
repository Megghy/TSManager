using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Newtonsoft.Json.Linq;
using PropertyChanged;

namespace TSManager.Data
{
    [AddINotifyPropertyChangedInterface]
    class ConfigData
    {
        public static List<ConfigData> ReadAllConfig()
        {
            var list = new List<ConfigData>();
            var files = Directory.GetFiles(Info.ConfigPath, "*.json");
            if (files.Any())
            {
                foreach (var filename in files)
                {
                    var shortName = System.IO.Path.GetFileName(filename);
                    try
                    {
                        var text = File.ReadAllText(filename);
                        if (Utils.TryParseJson(text, out var jobj))
                        {
                            if (jobj.TryGetValue("Settings", out var temp) && jobj.Count == 1) list.Add(new ConfigData(shortName, filename, text, (JObject)jobj["Settings"]));
                            else list.Add(new ConfigData(shortName, filename, text, jobj));
                        }
                        else
                        {
                            list.Add(new ConfigData(shortName, filename, text, null));
                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.Notice($"读取配置文件: {shortName} 时失败.\n{ex}", HandyControl.Data.InfoType.Error);
                    }
                }
            }
            return list;
        }
        public ConfigData(string name, string path, string text, JObject jobj)
        {
            Name = name;
            Path = path;
            JsonData = jobj;
            Text = text;
            ErrorCheck();
        }
        public void ErrorCheck() => Error = !Utils.TryParseJson(Text, out _);
        public string Name { get; set; }
        public string Path { get; set; }
        public JObject JsonData { get; set; }
        public string Text { get; set; }
        [AlsoNotifyFor("Status", new string[] { "StatusColor" })]
        public bool Error { get; set; }
        public string Status { get { return Error ? "无效" : "有效"; } set { } }
        public Brush StatusColor { get { return Error ? Color.FromRgb(253, 86, 86).ToBrush() : Color.FromRgb(178, 223, 120).ToBrush(); } set { } }

    }
    public class ConfigProcess : ICompletionData
    {
        public ConfigProcess(string text)
        {
            this.Text = text;
        }

        public ImageSource Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return this.Text; }
        }

        public object Description
        {
            get { return "Description for " + this.Text; }
        }

        public double Priority => 0;

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }
}
