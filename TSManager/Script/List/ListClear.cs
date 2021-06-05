using System.Collections.Generic;

namespace ScratchNet
{
    public class ListClear : Expression
    {
        public override string ReturnType => "void";
        public Expression List { get; set; }
        public override Descriptor Descriptor => new()
        {
            new TextItemDescriptor(this, "清空列表 [", true),
            new ExpressionDescriptor(this, "List", "any") { NothingAllowed = true },
            new TextItemDescriptor(this, "]"),
        };

        public override string Type => "ListClear";

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            if (List == null)
                return Completion.Exception("无效的数组", this);
            Completion c = List.Execute(enviroment);
            if (!c.IsValue)
                return c;
            if (!(c.ReturnValue is List<object>))
                return Completion.Exception("此处仅允许列表类型的数据", List);
            ((List<object>)c.ReturnValue).Clear();
            return Completion.Void;
        }
    }
}