using ScratchNet;
using TShockAPI;

namespace TSManager.Script
{
    class CheckPermission : Expression
    {
        public override string ReturnType => "bool";
        public Expression Target { get; set; }
        public Expression Permission { get; set; }
        public override Descriptor Descriptor
        {
            get
            {
                Descriptor desc = new();
                desc.Add(new TextItemDescriptor(this, "玩家: "));
                desc.Add(new ExpressionDescriptor(this, "Target", "GetPlayer"));
                desc.Add(new TextItemDescriptor(this, " 拥有权限: "));
                desc.Add(new ExpressionDescriptor(this, "Permission", "any"));
                return desc;
            }
        }

        public override string Type => "CheckPermission";

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            if (Permission.TryExcute<string>(enviroment, out var permission) && Target.TryExcute<TSPlayer>(enviroment, out var target))
            {
                return target is null ? (new("无效的玩家对象", CompletionType.Exception)) : (new(target.HasPermission(permission)));
            }
            return new(false);
        }
    }
}
