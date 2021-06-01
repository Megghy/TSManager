using System;
using System.Linq;
using ScratchNet;
using static System.Net.Mime.MediaTypeNames;

namespace TSManager.Script
{
    class GetPlayer : Expression
    {
        public override string ReturnType => "tsplayer";

        public Expression Name { get; set; }

        public override Descriptor Descriptor
        {
            get
            {
                Descriptor desc = new();
                desc.Add(new TextItemDescriptor(this, "获取玩家: "));
                desc.Add(new ExpressionDescriptor(this, "Name", "any"));
                return desc;
            }
        }

        public override string Type => "GetPlayer";

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            if (Name.TryExcute<string>(enviroment, out var name))
            {
                var plr = TShockAPI.TShock.Players.SingleOrDefault(p => p != null && p.Name == name);
                return plr == null ? Completion.Void : new(plr);
            }
            return Completion.Void;
        }
    }
}