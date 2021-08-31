using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using ScratchNet;
using Expression = ScratchNet.Expression;

namespace TSManager.Data
{
    public class ScriptData : Class
    {
        public static List<ScriptData> GetAllScripts()
        {
            if (!Directory.Exists(Info.Path + "Scripts"))
                Directory.CreateDirectory(Info.Path + "Scripts");
            var list = new List<ScriptData>();
            var files = Directory.GetFiles(Info.Path + "Scripts", "*.tsms");
            if (files.Any())
            {
                foreach (var filename in files)
                {
                    if (Read(filename) is { } data)
                    {
                        list.Add(data);
                    }
                }
            }
            return list;
        }
        public static ScriptData Read(string path)
        {
            try
            {
                XmlDocument xmlDoc = new();
                xmlDoc.Load(path);
                XmlNode scriptInfo = xmlDoc.SelectSingleNode("/Script/Info");
                XmlNode scriptNode = xmlDoc.SelectSingleNode("/Script/Class");
                ScriptData tempScript = Serialization.Load(path) as ScriptData;
                tempScript.Command = scriptInfo.Attributes["Command"].Value;
                tempScript.FilePath = path;
                tempScript.Name = scriptInfo.Attributes["Name"].Value;
                tempScript.Author = scriptInfo.Attributes["Author"].Value;
                tempScript.Description = scriptInfo.Attributes["Description"].Value;
                tempScript.Version = Version.Parse(scriptInfo.Attributes["Version"].Value);
                tempScript.ID = Guid.Parse(scriptInfo.Attributes["ID"].Value);
                tempScript.TriggerCondition = (Triggers)Enum.Parse(typeof(Triggers), scriptInfo.Attributes["TriggerCondition"].Value);
                tempScript.Enable = bool.Parse(scriptInfo.Attributes["Enable"].Value);
                return tempScript;
            }
            catch (Exception ex)
            {
                Utils.Notice($"读取脚本: {Path.GetFileName(path)} 时失败.\n{ex}", HandyControl.Data.InfoType.Error);
                return null;
            }
        }
        public ScriptData(string name)
        {
            Name = name;
            Author = "未知";
            Description = "无";
            Version = new(1, 0, 0, 0);
            ID = new();
            Positions = new Dictionary<object, System.Windows.Point>();
            Variables = new List<Variable>();
            Functions = new List<Function>();
            Handlers = new List<ScratchNet.EventHandler>();
            Expressions = new List<Expression>();
            BlockStatements = new List<BlockStatement>();
        }
        public ScriptData()
        {
            Name = "NewScript";
            Author = "未知";
            Description = "无";
            Version = new(1, 0, 0, 0);
            ID = new();
            Positions = new Dictionary<object, System.Windows.Point>();
            Variables = new List<Variable>();
            Functions = new List<Function>();
            Handlers = new List<ScratchNet.EventHandler>();
            Expressions = new List<Expression>();
            BlockStatements = new List<BlockStatement>();
        }
        public enum Triggers
        {
            None,
            Command,
            PlayerJoin,
            PlayerLeave,
            PlayerChat,
            PlayerDead,
        }
        public bool Enable { get; set; }
        public Triggers TriggerCondition { get; set; }
        public string Command { get; set; }
        public string FilePath { get; set; }
        public string FileName { get => Path.GetFileNameWithoutExtension(FilePath); set { } }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public Version Version { get; set; }
        public Guid ID { get; set; }
        public int FunctionCount => Expressions.Count + BlockStatements.Count;
        public Dictionary<object, System.Windows.Point> Positions
        {
            get;
            set;
        }
        public List<Variable> Variables
        {
            get;
            set;
        }

        public List<Function> Functions
        {
            get;
            set;
        }

        public List<ScratchNet.EventHandler> Handlers
        {
            get;
            set;
        }
        public List<Expression> Expressions
        {
            get;
            set;
        }
        public List<BlockStatement> BlockStatements
        {
            get;
            set;
        }
        public List<string> Imports { get; set; } = new List<string>();
    }
}
