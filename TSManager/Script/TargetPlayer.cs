using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScratchNet;
using TShockAPI;

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
            return new(enviroment.GetValue<TSPlayer>("TargetPlayer"));
        }
    }
}
