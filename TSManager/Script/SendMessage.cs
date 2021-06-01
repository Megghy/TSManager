using ScratchNet;
using TShockAPI;

namespace TSManager.Script
{
    internal class SendMessage : Statement
    {
        public override string ReturnType => "void";
        public Expression Target { get; set; }
        public Expression Text { get; set; }

        public override Descriptor Descriptor
        {
            get
            {
                Descriptor desc = new();
                desc.Add(new TextItemDescriptor(this, "发送消息: "));
                desc.Add(new ExpressionDescriptor(this, "Text", "any"));
                desc.Add(new TextItemDescriptor(this, " 给: "));
                desc.Add(new ExpressionDescriptor(this, "Target", "GetPlayer") { IsOnlyNumberAllowed = false });
                return desc;
            }
        }

        public override string Type => "SendMessage";

        public override BlockDescriptor BlockDescriptor => null;

        public override bool IsClosing => false;

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            if (Text.TryExcute<string>(enviroment, out var text) && Target.TryExcute<TSPlayer>(enviroment, out var target))
            {
                target.SendInfoMessage(text);
            }
            return Completion.Void;
        }
    }
}
