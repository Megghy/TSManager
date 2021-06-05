using ScratchNet;
using TShockAPI;

namespace TSManager.Script
{
    class ExcuteCommandInConsole : Statement
    {
        public override string ReturnType => "void";
        public Expression Command { get; set; }
        public Expression Target { get; set; }
        public override Descriptor Descriptor
        {
            get
            {
                Descriptor desc = new();
                desc.Add(new TextItemDescriptor(this, "控制台执行命令: "));
                desc.Add(new ExpressionDescriptor(this, "Command", "string"));
                desc.Add(new TextItemDescriptor(this, " <玩家: "));
                desc.Add(new ExpressionDescriptor(this, "Target", "any"));
                desc.Add(new TextItemDescriptor(this, " >"));
                return desc;
            }
        }

        public override string Type => "ExcuteCommandInConsole";

        public override BlockDescriptor BlockDescriptor => null;

        public override bool IsClosing => false;

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            if (Command.TryExcute<string>(enviroment, out var command))
            {
                if (Target.TryExcute<TSPlayer>(enviroment, out var target)) command = command.Replace("{name}", target.Name);
                Commands.HandleCommand(TSPlayer.Server, command.StartsWith(Commands.Specifier) ? command : Commands.Specifier + command);
            }
            return Completion.Void;
        }
    }
}
