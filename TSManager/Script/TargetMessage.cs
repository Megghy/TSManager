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
