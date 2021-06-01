using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using ScratchNet;
using TShockAPI;
using TSManager.Data;
using TSManager.Modules;

namespace TSManager
{
    public static class ScriptManager
    {
        internal static void LoadAllBlock()
        {

            //下面一大堆基本都是复制粘贴的, 有些本地化的东西懒得改, 将就将就
            TSMMain.GUI.Script_Editor.IsLibraryEnabled = false;
            List<ScriptStepGroup> toolbar = new List<ScriptStepGroup>();
            TSMMain.GUI.Script_Editor.Register((Color)ColorConverter.ConvertFromString("#B1F1D4"), typeof(StringExpression), typeof(IndexOfStringExpression), typeof(LastIndexOfStringExpression), typeof(SubStringExpression), typeof(StringLengthExpression), typeof(ParseLongExpression), typeof(ParseIntExpression), typeof(ParseFloatExpression), typeof(ParseDoubleExpression)); //数据操作颜色
            TSMMain.GUI.Script_Editor.Register((Color)ColorConverter.ConvertFromString("#93B5E4"), typeof(PrintLnStatement), typeof(PrintStatement), typeof(ReadLineExpression), typeof(ReadExpression), typeof(ClearStatement)); //控制台操作颜色
            TSMMain.GUI.Script_Editor.Register<IfStatement, TrueExpression, FalseExpression>((Color)ColorConverter.ConvertFromString("#9CB5BB")); //布尔值的颜色
            TSMMain.GUI.Script_Editor.Register((Color)ColorConverter.ConvertFromString("#D4E3AF"), typeof(NewArrayExpression), typeof(ArrayValueExpression),
                typeof(NewArray2Expression), typeof(Array2ValueExpression),
                typeof(ArrayLengthExpression));  //数组的颜色
            TSMMain.GUI.Script_Editor.Register((Color)ColorConverter.ConvertFromString("#EFBC94"), typeof(VariableDeclarationExpression), typeof(Identifier)); //变量的颜色
            TSMMain.GUI.Script_Editor.Register<Script.ExcuteCommand, Script.ExcuteCommandInConsole, Script.GetPlayer, Script.GetPlayerBag, Script.SendMessage, Script.CheckPermission, Script.TargetPlayer>((Color)ColorConverter.ConvertFromString("#EABEB0")); //玩家操作的颜色
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
                Name = "流程",
                Types = new()
                {
                    new Label() { Content = Properties.Resources.ExpressionStatementCategory },
                    new ScriptStep(new ExpressionStatement(), true, Properties.Resources.ExpressionStatementDescription + "如设置一条变量"),
                    //如果
                    new Label() { Content = Properties.Resources.IfCategory, ToolTip = Properties.Resources.IfDescription },
                    new ScriptStep(new IfStatement(), true, Properties.Resources.IfStatementDescription),
                    new ScriptStep(new IfStatement() { Alternate = new BlockStatement() }, true, Properties.Resources.IfElseStatementDescription),

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
                    new ScriptStep(new ForStatement(), true, "for循环语句.\n 第一格为第一次进入循环时将会调用的语句, 可以在此处声明临时变量. \n第二格为每次循环完成后将会进行的逻辑运算, 如果为 true将会继续, 反之退出循环. \n第三格为每次循环完成后将会执行的语句."),
                    new ScriptStep(new WhileStatement(), true, "每次循环完成后比较上方提供的表达式, 为true则继续循环, 反之退出. 使用此语句要注意退出循环的条件, 否则容易造成死循环"),
                    new ScriptStep(new DoStatement(), true, Properties.Resources.DoWhileDescription),
                    new ScriptStep(new BreakStatement(), true, "无视条件跳出正在运行的循环语句. (如有多层循环嵌套则只会跳出一层"),
                    new ScriptStep(new ContinueStatement(), true, "跳过循环中的其他语句, 直接跳到循环语句开头或结尾的逻辑比较处.")
                }
            });
            toolbar.Add(new ScriptStepGroup()
            {
                Name = "比较",
                Types = new()
                {
                    new Label() { Content = "比较" },
                    new ScriptStep(new BinaryExpression() { ValueType = "boolean", Operator = Operator.Equal }, true, Properties.Resources.BinaryExpressionDescription),//compare operator
                    new ScriptStep(new BinaryExpression() { ValueType = "boolean", Operator = Operator.And }, true, Properties.Resources.BinaryExpressionDescription),//logical operator
                    new ScriptStep(new NotExpression(), true, Properties.Resources.NotExpressionDescription),
                    new ScriptStep(new NullExpression(), true),
                }
            });
            toolbar.Add(new ScriptStepGroup()
            {
                Name = "数据操作",
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
                    new ScriptStep(new BinaryExpression(), true, Properties.Resources.BinaryExpressionDescription),
                    new ScriptStep(new UpdateExpression(), true, Properties.Resources.UpdateExpressionDescription),
                    new ScriptStep(new UpdateExpression() { IsPrefix = true }, true, Properties.Resources.UpdateExpressionDescription),

                    new Label() { Content = "数组" },
                    new ScriptStep(new ArrayValueExpression(), true, Properties.Resources.ArrayValueDescription),
                    new ScriptStep(new Array2ValueExpression(), true, Properties.Resources.ArrayValueDescription),
                    new ScriptStep(new ArrayLengthExpression(), true, Properties.Resources.ArrayLengthDescription),
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

                    new Label() { Content = Properties.Resources.ArrayCollection },
                    new ScriptStep(new NewArrayExpression(), true, Properties.Resources.NewArrayDescription),
                    new ScriptStep(new NewArray2Expression(), true, Properties.Resources.NewArray2Description)
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
            toolbar.Add(new ScriptStepGroup()
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
            });
            toolbar.Add(new ScriptStepGroup()
            {
                Name = "玩家操作",
                Types = new()
                {
                    new Label() { Content = "预置变量" },
                    new ScriptStep(new Script.TargetPlayer(), true, "引发,"),
                    new Label() { Content = "玩家信息" },
                    new ScriptStep(new Script.GetPlayer(), true, "获取指定名称的玩家对象"),
                    new ScriptStep(new Script.SendMessage(), true, "向指定玩家发送指定消息"),
                    new ScriptStep(new Script.GetPlayerBag(), true, "获取指定玩家的背包, 类型为array"),
                    new ScriptStep(new Script.ExcuteCommand(), true, "让目标玩家执行一条命令\r\n(命令中的 {name} 将被替换为玩家名称)"),
                    new ScriptStep(new Script.ExcuteCommandInConsole(), true, "在控制台执行一条命令\r\n(命令中的 {name} 将被替换为玩家名称)"),
                    new ScriptStep(new Script.CheckPermission(), true, "检查玩家是否拥有指定权限, 返回bool值"),
                }
            });
            toolbar.Add(GetCommandsFromLib(new MathLibary())); //引入数学拓展库
            toolbar.Add(GetCommandsFromLib(new CollectionLibary())); //引入集合拓展库
            toolbar.Add(GetCommandsFromLib(new DataStructureLibrary())); //引入数据结构拓展库
            toolbar.Add(GetCommandsFromLib(new ThreadCollection())); //引入线程拓展库
            TSMMain.GUI.Script_Editor.SetToolBar(toolbar);
        }
        static ScriptStepGroup GetCommandsFromLib(Library l)
        {
            ScriptStepGroup g = new ScriptStepGroup();
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
        public static void ChangeScript(ScriptData script)
        {
            TSMMain.GUI.Script_Editor.Script = script;
            TSMMain.GUI.Script_Tab.SelectedIndex = 1;
        }
        public static void ChangeTriggerCondition(ScriptData script, ScriptData.Triggers type)
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
                scriptInfo.SetAttribute("Name", script.Name);
                scriptInfo.SetAttribute("Author", script.Author);
                scriptInfo.SetAttribute("Description", script.Description);
                scriptInfo.SetAttribute("Version", script.Version.ToString());
                scriptInfo.SetAttribute("ID", script.ID.ToString());
                scriptInfo.SetAttribute("TriggerCondition", script.TriggerCondition.ToString());
                scriptInfo.SetAttribute("Enable", script.Enable.ToString());
                scriptNode.AppendChild(scriptInfo);
                xmlDocument.AppendChild(scriptNode);
                xmlDocument.Save(Info.Path + "\\Scripts\\" + script.Name + ".tsms");
                if (notice) Utils.Notice("已保存修改", HandyControl.Data.InfoType.Success);
                //Info.Scripts.SingleOrDefault(s => s.ID = script.ID)?
            }
            catch { }
        }
        public static void Log(object text)
        {
            TSMMain.AddText($"[ScriptManager] ", Color.FromRgb(177, 241, 241));
            TSMMain.AddLine(text);
        }
        public static void Create(string name, string author, string description, string version)
        {
            var path = $"{Info.Path}\\Scripts\\{name}.tsms";
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
                        scriptInfo.SetAttribute("Name", name);
                        scriptInfo.SetAttribute("Author", author);
                        scriptInfo.SetAttribute("Description", description);
                        scriptInfo.SetAttribute("Version", _version.ToString());
                        scriptInfo.SetAttribute("ID", Guid.NewGuid().ToString());
                        scriptInfo.SetAttribute("TriggerCondition", ScriptData.Triggers.None.ToString());
                        scriptInfo.SetAttribute("Enable", true.ToString());
                        xmlDoc.Save(path);
                        var script = ScriptData.Read(path);
                        TSMMain.GUI.Script_Editor.Script = script;
                        TSMMain.GUI.Script_Editor.Print();
                        TSMMain.GUI.Script_Create_Drawer.IsOpen = false;
                    }
                    Info.Scripts.Add(new() { Name = name, Description = description, Version = _version, Author = author });
                    Utils.Notice($"成功创建脚本 {name}", HandyControl.Data.InfoType.Success);
                }
                catch { Utils.Notice($"创建脚本 {name} 失败", HandyControl.Data.InfoType.Error); }
            }
            else
            {
                Utils.Notice("无效的版本号.\r\n应为x.x.x.x, 可减少位数", HandyControl.Data.InfoType.Error);
            }
        }
        /// <summary>
        /// 由hooks调用的脚本运行
        /// </summary>
        /// <param name="type"></param>
        internal static void ExcuteScript(ScriptData.Triggers type, TSPlayer player = null)
        {
            var scripts = Info.Scripts.Where(s => s.Enable && s.TriggerCondition == type);
            if(scripts.Any()) Log($"[{player?.Name}] 事件: {type}, 执行脚本 {string.Join(", ", scripts.Select(s => s.Name))}");
            else Log($"[{player?.Name}] 事件: {type}, 无可执行脚本");
            scripts.ForEach(s =>
            {
                s.Variables.Where(v => v.Name == "TargetPlayer").ForEach(v => v.Value = player);
                s.Excute(player);
            });
        }
        #region 脚本运行
        public async static void Excute(this ScriptData script, TSPlayer player = null)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (script.Functions.Any(f => f.Name == "main"))
                    {
                        Stack<Node> stackTrace = new();
                        stackTrace.Clear();
                        ExecutionEnvironment engine = new();
                        if (!engine.HasValue("TargetPlayer")) engine.RegisterValue("TargetPlayer", player);
                        engine.SetValue("TargetPlayer", player);
                        //Editor.IsEnabled = false;
                        engine.ExecutionCompleted += Engine_ExecutionCompleted;
                        engine.ExecutionAborted += Engine_ExecutionAborted;
                        engine.ExecuteAsync(script);
                    }
                    else Utils.Notice($"未找到脚本 {script.Name} 的入口点<main>函数");
                }
                catch (Exception ex)
                {
                    Log($"脚本 {script.Name} 运行时发生错误.\r\n{ex}");
                }
            });
        }
        static void Engine_ExecutionCompleted(object e, object arg)
        {

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
                TSMMain.AddLine($"[ScriptManager] 脚本异常终止. 发生于 {arg.Location}, 错误信息: {arg.ReturnValue}");
            }
        }
        #endregion
    }
}
