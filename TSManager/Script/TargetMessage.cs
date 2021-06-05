using ScratchNet;
using static TSManager.ScriptManager;

namespace TSManager.Script
{
    class TargetMessage : Expression
    {
        public override string ReturnType => "string";

        public override Descriptor Descriptor
        {
            get
            {
                Descriptor desc = new();
                desc.Add(new TextItemDescriptor(this, "玩家发送的消息"));
                return desc;
            }
        }

        public override string Type => "TargetMessage";

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            return new(enviroment.GetValue<ScriptExcuteArgs>("ScriptExcuteArgs")?.Message);
        }
    }
}
