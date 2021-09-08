using HandyControl.Controls;
using ScratchNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using TShockAPI;
using TSManager.Data;
using TSManager.Modules;
using TSManager.Script;
using static TSManager.Data.ScriptData;

namespace TSManager
{
    public static partial class ScriptManager
    {
        public static ScriptData SelectedScriptData => TSMMain.GUI.Script_List.SelectedItem as ScriptData;
        internal static void Init()
        {
            LoadAllBlock(); //加载脚本编辑器
            Log("正在加载所有脚本");
            Info.Scripts = new(ScriptData.GetAllScripts());
            Log($"共载入 {Info.Scripts.Count} 条脚本");
            TSMMain.GUI.Script_List.ItemsSource = Info.Scripts;
            TSMMain.GUI.Script_TriggerCondition.ItemsSource = Enum.GetValues(typeof(ScriptData.Triggers)).Cast<ScriptData.Triggers>();
        }
        internal static void LoadAllBlock()
        {

            //下面一大堆基本都是复制粘贴的, 有些本地化的东西懒得改, 将就将就
            TSMMain.GUI.Script_Editor.IsLibraryEnabled = false;
            List<ScriptStepGroup> toolbar = new();
            TSMMain.GUI.Script_Editor.Register((Color)ColorConverter.ConvertFromString("#B1F1D4"),
                typeof(StringExpression),
                typeof(Conditional),
                typeof(IndexOfStringExpression),
                typeof(LastIndexOfStringExpression),
                typeof(SubStringExpression),
                typeof(StringLengthExpression),
                typeof(ParseLongExpression),
                typeof(ParseIntExpression),
                typeof(ParseFloatExpression),
                typeof(ParseDoubleExpression)
                ); //数据操作颜色
            /* TSMMain.GUI.Script_Editor.Register((Color)ColorConverter.ConvertFromString("#93B5E4"),
                 typeof(PrintLnStatement), 
                 typeof(PrintStatement), 
                 typeof(ReadLineExpression),
                 typeof(ReadExpression),
                 typeof(ClearStatement)
                 ); //控制台操作颜色*/
            TSMMain.GUI.Script_Editor.Register<IfEx, TrueExpression, FalseExpression>((Color)ColorConverter.ConvertFromString("#9CB5BB")); //布尔值的颜色
            TSMMain.GUI.Script_Editor.Register((Color)ColorConverter.ConvertFromString("#D4E3AF"),
                typeof(NewArrayExpression),
                typeof(ArrayValueExpression),
                typeof(NewArray2Expression),
                typeof(Array2ValueExpression),
                typeof(ArrayLengthExpression),
                typeof(ListInsertExpression),
                typeof(ListAddExpression),
                typeof(ListRemoveAtExpression),
                typeof(ListRemoveExpression),
                typeof(ListValueExpression),
                typeof(NewListExpression),
                typeof(ListClear)
                );  //数组的颜色
            TSMMain.GUI.Script_Editor.Register((Color)ColorConverter.ConvertFromString("#EFBC94"),
                typeof(VariableDeclarationExpression),
                typeof(Identifier)
                ); //变量的颜色
            TSMMain.GUI.Script_Editor.Register((Color)ColorConverter.ConvertFromString("#EABEB0"),
                typeof(ExcuteCommand),
                new[] {
                    typeof(ExcuteCommandInConsole),
                    typeof(GetPlayer),
                    typeof(GetPlayerBag),
                    typeof(SendMessage),
                    typeof(CheckPermission),
                    typeof(TargetPlayer),
                    typeof(TargetPlayerName),
                    typeof(TargetMessage)
                }); //玩家操作的颜色
            toolbar.Add(new ScriptStepGroup()
            {
                Name = Properties.Resources.CommentCollection,
                Types = new()
                {
                    new Label() { Content = Properties.Resources.CommentCategory, ToolTip = Properties.Resources.CommentDescription },
                    new ScriptStep(new CommentStatement(), true, Properties.Resources.CommentDescription1),
                    new ScriptStep(new CommentStatement() { AllowMultiLine = true }, true, Properties.Resources.CommentDescription2)
                }
            });

            toolbar.Add(new ScriptStepGroup()
            {
                Name = "逻辑",
                Types = new()
                {
                    new Label() { Content = Properties.Resources.ExpressionStatementCategory },
                    new ScriptStep(new ExpressionStatement(), true, Properties.Resources.ExpressionStatementDescription + "如设置一条变量"),
                    //如果
                    new Label() { Content = Properties.Resources.IfCategory, ToolTip = Properties.Resources.IfDescription },
                    new ScriptStep(new IfEx(), true, "对填入的bool值进行判断, 如果为真则执行内部脚本"),
                    new ScriptStep(new IfEx() { Alternate = new BlockStatement() }, true, Properties.Resources.IfElseStatementDescription),

                    new Label() { Content = "比较" },
                    new ScriptStep(new Conditional() { ValueType = "boolean", Operator = Operator.Equal }, true, Properties.Resources.BinaryExpressionDescription),//compare operator
                    new ScriptStep(new Conditional() { ValueType = "boolean", Operator = Operator.And }, true, Properties.Resources.BinaryExpressionDescription),//logical operator
                    new ScriptStep(new NotExpression(), true, Properties.Resources.NotExpressionDescription),
                    new ScriptStep(new NullExpression(), true),

                    //返回
                    new Label() { Content = Properties.Resources.ReturnCategory },
                    new ScriptStep(new ReturnStatement(), true, "退出正在运行的函数, 忽略其后所有语句."),
                    new ScriptStep(new ReturnStatement() { Expression = new Literal() }, true, "退出正在运行的函数, 并返回指定变量给调用函数."),
                    //异常
                    new Label() { Content = Properties.Resources.ExceptionCategory },
                    new ScriptStep(new TryStatement(), true, "捕获所包含的语句发生的所有错误, 并输出在下方变量中."),
                    //new ScriptStep(new TryStatement(){Finally=new BlockStatement()},true, "try-catch with finally statement")
                }
            });
            toolbar.Add(new ScriptStepGroup()
            {
                Name = "循环",
                Types = new()
                {
                    new ScriptStep(new ForStatement(), true, "for循环语句.\r\n 第一格为第一次进入循环时将会调用的语句, 可以在此处声明临时变量. \r\n第二格为每次循环完成后将会进行的逻辑运算, 如果为 true将会继续, 反之退出循环. \r\n第三格为每次循环完成后将会执行的语句."),
                    new ScriptStep(new WhileStatement(), true, "每次循环完成后比较上方提供的表达式, 为true则继续循环, 反之退出. 使用此语句要注意退出循环的条件, 否则容易造成死循环"),
                    new ScriptStep(new DoStatement(), true, Properties.Resources.DoWhileDescription),
                    new ScriptStep(new BreakStatement(), true, "无视条件跳出正在运行的循环语句. (如有多层循环嵌套则只会跳出一层"),
                    new ScriptStep(new ContinueStatement(), true, "跳过循环中的其他语句, 直接跳到循环语句开头或结尾的逻辑比较处.")
                }
            });
            toolbar.Add(new ScriptStepGroup()
            {
                Name = "数组及列表",
                Types = new()
                {
                    new Label() { Content = "数组<Array>" },
                    new ScriptStep(new ArrayValueExpression(), true, Properties.Resources.ArrayValueDescription),
                    new ScriptStep(new Array2ValueExpression(), true, Properties.Resources.ArrayValueDescription),
                    new ScriptStep(new ArrayLengthExpression(), true, Properties.Resources.ArrayLengthDescription),

                    new Label() { Content = "列表<List>" },
                    new ScriptStep(new NewListExpression(), true, "创建一个新的列表(List)类型的对象 <List>"),
                    new ScriptStep(new ListValueExpression(), true, "获取指定位置处的对象\r\n注意: 列表是从0开始 <Object>"),
                    new ScriptStep(new ListAddExpression(), true, "向指定列表的末尾处添加一个成员 <无类型>"),
                    new ScriptStep(new ListRemoveExpression(), true, "移除指定列表内的指定成员 <无类型>"),
                    new ScriptStep(new ListRemoveAtExpression(), true, "移除指定列表内指定位置的成员 <无类型>\r\n注意: 列表是从0开始"),
                    new ScriptStep(new ListInsertExpression(), true, "在指定列表的指定位置处添加一个成员\r\n注意: 列表是从0开始 <无类型>"),
                    new ScriptStep(new ListClear(), true, "清除列表内的所有成员 <无类型>"),
                }
            });
            toolbar.Add(new ScriptStepGroup()
            {
                Name = "数据",
                Types = new()
                {
                    new Label() { Content = Properties.Resources.VariableAssignmentCategory },
                    new ExpressionStatement() { Expression = new AssignmentExpression() },
                    new ScriptStep(new AssignmentExpression(), true, Properties.Resources.VariableAssignmentDescription),

                    new Label() { Content = "字符串" },
                    new ScriptStep(new StringExpression(), true, Properties.Resources.NewStringDescription),
                    new ScriptStep(new StringLengthExpression(), true, Properties.Resources.StringLengthDescription),
                    new ScriptStep(new IndexOfStringExpression(), true, Properties.Resources.StringIndexDescription),
                    new ScriptStep(new LastIndexOfStringExpression(), true, Properties.Resources.StringLastIndexDescription),
                    new ScriptStep(new SubStringExpression(), true, Properties.Resources.StringSubDescription),

                    new Label() { Content = "运算" },
                    new ScriptStep(new Conditional(), true, Properties.Resources.BinaryExpressionDescription),
                    new ScriptStep(new UpdateExpression(), true, Properties.Resources.UpdateExpressionDescription),
                    new ScriptStep(new UpdateExpression() { IsPrefix = true }, true, Properties.Resources.UpdateExpressionDescription),
                }
            });
            toolbar.Add(new ScriptStepGroup()
            {
                Name = "变量",
                Types = new()
                {
                    new Label() { Content = "声明变量" },
                    "CreateVariable",
                    new Label() { Content = Properties.Resources.VariableDefCategory }, //声明变量
                    new ScriptStep(new StringExpression(), true, Properties.Resources.NewStringDescription),
                    new ExpressionStatement() { Expression = new VariableDeclarationExpression() { CanAssignValue = false } },
                    new ExpressionStatement() { Expression = new VariableDeclarationExpression() },
                    new ScriptStep(new VariableDeclarationExpression() { CanAssignValue = false }, true, Properties.Resources.VariableDecDescription),
                    new ScriptStep(new VariableDeclarationExpression(), true, Properties.Resources.VariableDecDescription2),


                    new Label() { Content = Properties.Resources.NullCategory },
                    new ScriptStep(new NullExpression(), true),

                    new Label() { Content = Properties.Resources.LogicValueCategory },
                    new ScriptStep(new TrueExpression(), true),
                    new ScriptStep(new FalseExpression(), true),

                    new Label() { Content = Properties.Resources.TypeConvertCategory },
                    new ScriptStep(new ParseIntExpression(), true, Properties.Resources.ParseIntDescription),
                    new ScriptStep(new ParseLongExpression(), true, Properties.Resources.ParseIntDescription),
                    new ScriptStep(new ParseFloatExpression(), true, Properties.Resources.ParseFloatDescription),
                    new ScriptStep(new ParseDoubleExpression(), true, Properties.Resources.ParseDoubleDescription),
                }
            });
            toolbar.Add(new ScriptStepGroup()
            {
                Name = Properties.Resources.FunctionCollection,
                Types = new()
                {
                    new Label() { Content = Properties.Resources.FunctionNewCategory },
                    "CreateFunction",
                    new Label() { Content = Properties.Resources.FunctionCallCategory }
                }
            });
            /*toolbar.Add(new ScriptStepGroup()
            {
                Name = Properties.Resources.IOCollection,
                Types = new()
                {
                    new Label() { Content = Properties.Resources.ConsoleCategory },
                    new ScriptStep(new PrintLnStatement(), true, Properties.Resources.PrintLnDescription),
                    new ScriptStep(new PrintStatement(), true, Properties.Resources.PrintDescription),
                    new ScriptStep(new ReadLineExpression(), true, Properties.Resources.ReadLnDescription),
                    new ScriptStep(new ReadExpression(), true, Properties.Resources.ReadDescripiton),
                    new ScriptStep(new ClearStatement(), true, Properties.Resources.ClearDescription)
                    //new Label(){Content="file"}
                }
            });*/
            toolbar.Add(new ScriptStepGroup()
            {
                Name = "玩家操作",
                Types = new()
                {
                    new Label() { Content = "预置变量" },
                    new ScriptStep(new TargetPlayer(), true, "引发脚本执行的玩家对象 <TSPlayer>\r\n仅在以下触发方式中有效:\r\n▪ PlayerJoin\r\n▪ PlayerLeave\r\n▪ PlayerChat\r\n▪ PlayerDead"),
                    new ScriptStep(new TargetPlayerName(), true, "引发脚本执行的玩家名 <string>\r\n仅在以下触发方式中有效:\r\n▪ PlayerJoin\r\n▪ PlayerLeave\r\n▪ PlayerChat\r\n▪ PlayerDead"),
                    new ScriptStep(new TargetMessage(), true, "玩家发送的消息 <string>\r\n仅在以下触发方式中有效:\r\n▪ PlayerChat"),

                    new Label() { Content = "玩家信息" },
                    new ScriptStep(new GetPlayer(), true, "获取指定名称的玩家对象 <TSPlayer>"),
                    new ScriptStep(new SendMessage(), true, "向指定玩家发送指定消息  <无类型>"),
                    new ScriptStep(new GetPlayerBag(), true, "获取指定玩家的背包 <List>"),
                    new ScriptStep(new ExcuteCommand(), true, "让目标玩家执行一条命令 <无类型>\r\n(命令中的 {name} 将被替换为玩家名称)"),
                    new ScriptStep(new ExcuteCommandInConsole(), true, "在控制台执行一条命令 <无类型>\r\n(命令中的 {name} 将被替换为玩家名称)"),
                    new ScriptStep(new CheckPermission(), true, "检查玩家是否拥有指定权限 <bool>"),
                }
            });
            toolbar.Add(GetCommandsFromLib(new MathLibary())); //引入数学拓展库
            toolbar.Add(GetCommandsFromLib(new DataStructureLibrary())); //引入数据结构拓展库
            toolbar.Add(GetCommandsFromLib(new ThreadCollection())); //引入线程拓展库
            TSMMain.GUI.Script_Editor.SetToolBar(toolbar);
        }
        private static ScriptStepGroup GetCommandsFromLib(Library l)
        {
            ScriptStepGroup g = new();
            g.Types = new List<object>();
            g.Name = l.Title;
            foreach (var gp in l)
            {
                g.Types.Add(new Label() { Content = gp.Name, ToolTip = gp.Description });
                foreach (var s in gp)
                {
                    g.Types.Add(new ScriptStep(s.Step, s.IsColorEditable, s.Description));
                }
            }
            return g;
        }
        public static void ChangeSelectScript(ScriptData script)
        {
            if (TSMMain.GUI.Script_Editor.IsModified)
                Growl.Ask($"脚本 {script.FileName} 已改动, 确定要离开吗?", result =>
                {
                    if (result) TSMMain.GUIInvoke(() =>
                    {
                        TSMMain.GUI.Script_Editor.Script = script;
                        TSMMain.GUI.Script_Tab.SelectedIndex = 1;
                    });  //这东西属实不咋好用
                    return true;
                });
            else
                TSMMain.GUIInvoke(() =>
                {
                    TSMMain.GUI.Script_Editor.Script = script;
                    TSMMain.GUI.Script_Tab.SelectedIndex = 1;
                });
        }
        public static void ChangeTriggerCondition(ScriptData script, Triggers type)
        {
            script.TriggerCondition = type;
            Utils.Notice("已修改触发条件", HandyControl.Data.InfoType.Success);
            script.Save(false);
        }
        public static void Save(this ScriptData script, bool notice = true)
        {
            try
            {
                MethodInfo method = typeof(Serialization).GetMethod("CreateClassNode", BindingFlags.IgnoreCase
                    | BindingFlags.NonPublic
                    | BindingFlags.Static);

                XmlDocument xmlDocument = new();
                xmlDocument.CreateXmlDeclaration("1.0", "utf-8", "yes");
                XmlNode scriptNode = xmlDocument.CreateElement("Script");
                scriptNode.AppendChild(method.Invoke(null, new object[] { script, xmlDocument }) as XmlNode);
                var scriptInfo = xmlDocument.CreateElement("Info");
                scriptInfo.SetAttribute("Command", script.Command ?? "");
                scriptInfo.SetAttribute("Name", script.Name);
                scriptInfo.SetAttribute("Author", script.Author);
                scriptInfo.SetAttribute("Description", script.Description);
                scriptInfo.SetAttribute("Version", script.Version.ToString());
                scriptInfo.SetAttribute("ID", script.ID.ToString());
                scriptInfo.SetAttribute("TriggerCondition", script.TriggerCondition.ToString());
                scriptInfo.SetAttribute("Enable", script.Enable.ToString());
                scriptNode.AppendChild(scriptInfo);
                xmlDocument.AppendChild(scriptNode);
                xmlDocument.Save(script.FilePath);
                if (notice) Utils.Notice("已保存修改", HandyControl.Data.InfoType.Success);
                //Info.Scripts.SingleOrDefault(s => s.ID = script.ID)?
            }
            catch { }
        }
        public static void Log(object text)
        {
            Utils.AddText($"[ScriptManager] ", Color.FromRgb(177, 241, 241));
            Utils.AddLine(text);
        }
        public static void Create(string name, string author, string description, string version)
        {
            var path = $"{Info.CurrentPath}Scripts\\{name}.tsms";
            if (name == string.Empty || author == string.Empty)
            {
                Utils.Notice("脚本名称和作者为必填项", HandyControl.Data.InfoType.Error);
                return;
            }
            else if (File.Exists(path))
            {
                Utils.Notice($"已存在名为 {name} 的脚本", HandyControl.Data.InfoType.Error);
                return;
            }
            else if (Version.TryParse(version, out var _version))
            {
                try
                {
                    using (var stream = new MemoryStream(Properties.Resources.NewScript))
                    {
                        XmlDocument xmlDoc = new();
                        xmlDoc.Load(stream);
                        var scriptInfo = (XmlElement)xmlDoc.SelectSingleNode("/Script/Info");
                        scriptInfo.SetAttribute("Command", "");
                        scriptInfo.SetAttribute("Name", name);
                        scriptInfo.SetAttribute("Author", author);
                        scriptInfo.SetAttribute("Description", description);
                        scriptInfo.SetAttribute("Version", _version.ToString());
                        scriptInfo.SetAttribute("ID", Guid.NewGuid().ToString());
                        scriptInfo.SetAttribute("TriggerCondition", Triggers.None.ToString());
                        scriptInfo.SetAttribute("Enable", true.ToString());
                        xmlDoc.Save(path);
                        var script = Read(path);
                        TSMMain.GUI.Script_Editor.Script = script;
                        TSMMain.GUI.Script_Create_Drawer.IsOpen = false;
                        Info.Scripts.Add(script);
                        Utils.Notice($"成功创建脚本 {name}", HandyControl.Data.InfoType.Success);
                    }
                }
                catch { Utils.Notice($"创建脚本 {name} 失败", HandyControl.Data.InfoType.Error); }
            }
            else
            {
                Utils.Notice("无效的版本号.\r\n应为x.x.x.x, 可减少位数", HandyControl.Data.InfoType.Error);
            }
        }
        public static void Del(ScriptData script)
        {
            Info.Scripts.Remove(script);
            TSMMain.GUI.Script_List.SelectedItem = null;
            TSMMain.GUI.Script_Editor.Script = null;
            var path = $"{Info.CurrentPath}Scripts\\{script.Name}.tsms";
            if (File.Exists(path)) File.Delete(path);
            Utils.Notice($"已删除脚本 {script.Name}", HandyControl.Data.InfoType.Success);
        }
    }
    /// <summary>
    /// 运行相关
    /// </summary>
    public static partial class ScriptManager
    {
        public class ScriptExcuteArgs
        {
            public ScriptExcuteArgs(Triggers type, TSPlayer target, string message)
            {
                Type = type;
                Target = target;
                Message = message;
            }
            public Triggers Type { get; set; }
            public TSPlayer Target { get; set; }
            public string Message { get; set; }
        }
        /// <summary>
        /// 由hooks调用的脚本运行
        /// </summary>
        /// <param name="type"></param>
        public static void ExcuteScript(ScriptExcuteArgs args)
        {
            var scripts = Info.Scripts.Where(s => s.Enable && s.TriggerCondition == args.Type);
            if (scripts.Any())
                Log($"<{args.Target?.Name}> 事件: {args.Type}, 执行脚本 {string.Join(", ", scripts.Select(s => s.Name))}");
            else
                Log($"事件: {args.Type}, 无可执行脚本");
            scripts?.ForEach(s =>
            {
                s.Variables.Where(v => v.Name == "TargetPlayer").ForEach(v => v.Value = args);
                s.ExcuteDirect(args);
            });
        }
        internal static void ExcuteDirect(this ScriptData script, ScriptExcuteArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    if (script.Functions.Any(f => f.Name == "main"))
                    {
                        Stack<Node> stackTrace = new();
                        stackTrace.Clear();
                        ExecutionEnvironment engine = new();
                        if (!engine.HasValue("ScriptExcuteArgs"))
                            engine.RegisterValue("ScriptExcuteArgs", args);
                        engine.SetValue("ScriptExcuteArgs", args);
                        //Editor.IsEnabled = false;
                        engine.ExecutionCompleted += Engine_ExecutionCompleted;
                        engine.ExecutionAborted += Engine_ExecutionAborted;
                        engine.ExecuteAsync(script);
                    }
                    else
                        Utils.Notice($"未找到脚本 {script.Name} 的入口点<main>函数");
                }
                catch (Exception ex)
                {
                    Log($"脚本 {script.Name} 初始化时发生错误.\r\n{ex}");
                }
            });
        }
        static void Engine_ExecutionCompleted(object e, object arg)
        {
            var script = e as ScriptData;
            //Utils.Notice($"脚本 {script.Name} 运行时发生错误");
            Log($"脚本 {script.Name} 执行完成.");
        }
        static void Engine_ExecutionAborted(object sender, Completion arg)
        {
            if (arg != null)
            {
                /*Dispatcher.InvokeAsync(() =>
                {
                    var ret = MessageBox.Show(Properties.Resources.ExceptionDuringRun + "\n" + arg.ReturnValue + "\n" + Properties.Resources.LocateException,
                        Properties.Resources.Exception, MessageBoxButton.YesNo, MessageBoxImage.Error);
                    if (ret == MessageBoxResult.Yes)
                    {
                        var editor = Editor.FindEditorFor(arg.Location);
                        if (editor == null)
                        {
                            var parent = Editor.FindParent(arg.Location);
                            editor = Editor.FindEditorFor(parent);
                        }
                        if (editor != null)
                        {
                            Console.WriteLine(editor);
                            Editor.Highlight(editor);
                            ButtonClearError.IsEnabled = true;
                        }
                    }
                    //var p = Editor.FindParent(arg.Location);
                    // var pe = Editor.FindEditorFor(p);
                });
                */
                var script = sender as ScriptData;
                //Utils.Notice($"脚本 {script.Name} 运行时发生错误");
                Log($"脚本 {script.Name} 异常终止. 发生于 {arg.Location}, 错误信息: {arg.ReturnValue}");
            }
        }
    }
}
