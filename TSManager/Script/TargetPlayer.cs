using ScratchNet;
using static TSManager.ScriptManager;

namespace TSManager.Script
{
    class TargetPlayer : Expression
    {
        public override string ReturnType => "tsplayer";

        public override Descriptor Descriptor
        {
            get
            {
                Descriptor desc = new();
                desc.Add(new TextItemDescriptor(this, "触发脚本的玩家"));
                return desc;
            }
        }

        public override string Type => "TargetPlayer";

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            return new(enviroment.GetValue<ScriptExcuteArgs>("ScriptExcuteArgs")?.Target);
        }
    }
}
