using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScratchNet;
using TShockAPI;
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
