using ScratchNet;
using static TSManager.ScriptManager;

namespace TSManager.Script
{
    class TargetPlayerName : Expression
    {
        public override string ReturnType => "string";

        public override Descriptor Descriptor
        {
            get
            {
                Descriptor desc = new();
                desc.Add(new TextItemDescriptor(this, "触发脚本的玩家名"));
                return desc;
            }
        }

        public override string Type => "TargetPlayerName";

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            return new(enviroment.GetValue<ScriptExcuteArgs>("ScriptExcuteArgs")?.Target.Name);
        }
    }
}
