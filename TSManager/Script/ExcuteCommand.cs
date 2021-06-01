using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScratchNet;
using static System.Net.Mime.MediaTypeNames;
using TShockAPI;

namespace TSManager.Script
{
    class ExcuteCommand : Statement
    {
        public override string ReturnType => "void";
        public Expression Target { get; set; }
        public Expression Command { get; set; }

        public override Descriptor Descriptor
        {
            get
            {
                Descriptor desc = new();
                desc.Add(new TextItemDescriptor(this, "执行命令: "));
                desc.Add(new ExpressionDescriptor(this, "Command", "string"));
                desc.Add(new TextItemDescriptor(this, " <玩家: "));
                desc.Add(new ExpressionDescriptor(this, "Target", "GetPlayer"));
                desc.Add(new TextItemDescriptor(this, " >"));
                return desc;
            }
        }

        public override string Type => "ExcuteCommand";

        public override BlockDescriptor BlockDescriptor => null;

        public override bool IsClosing => false;

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            if (Command.TryExcute<string>(enviroment, out var command) && Target.TryExcute<TSPlayer>(enviroment, out var target))
            {
                command = command.Replace("{name}", target.Name);
                Commands.HandleCommand(target, command.StartsWith(Commands.Specifier) ? command : Commands.Specifier + command);
            }
            return Completion.Void;
        }
    }
}
