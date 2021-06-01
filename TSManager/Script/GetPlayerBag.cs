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
    class GetPlayerBag : Expression
    {
        public override string ReturnType => "list";
        public Expression Target { get; set; }
        public override Descriptor Descriptor
        {
            get
            {
                Descriptor desc = new();
                desc.Add(new TextItemDescriptor(this, "获取 "));
                desc.Add(new ExpressionDescriptor(this, "Target", "GetPlayer"));
                desc.Add(new TextItemDescriptor(this, " 的背包"));
                return desc;
            }
        }

        public override string Type => "GetPlayerBag";

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            if (Target.TryExcute<TSPlayer>(enviroment, out var target))
            {
                return new(target.PlayerData.inventory);
            }
            return Completion.Void;
        }
    }
}
