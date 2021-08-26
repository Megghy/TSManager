using System.Collections.Generic;

namespace ScratchNet
{
    public class ListAddExpression : Expression
    {
        public Expression List { get; set; }
        public Expression Value { get; set; }
        public override string ReturnType => "any";

        public override Descriptor Descriptor => new()
        {
            new TextItemDescriptor(this, "为列表 ["),
            new ExpressionDescriptor(this, "List", "any") { NothingAllowed = true },
            new TextItemDescriptor(this, "] 添加成员 ["),
            new ExpressionDescriptor(this, "Value", "any"),
            new TextItemDescriptor(this, "] ")
        };

        public override string Type => "ListAddExpression";

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            if (List == null || Value == null)
                return Completion.Exception("Null Exception", this);
            Completion c = List.Execute(enviroment);
            if (!c.IsValue)
                return c;
            if (!(c.ReturnValue is List<object>))
                return Completion.Exception("Only list value is accepted here", List);
            Completion v = Value.Execute(enviroment);
            if (!v.IsValue)
                return v;
            List<object> stack = c.ReturnValue as List<object>;
            stack.Add(v.ReturnValue);
            return new Completion(v.ReturnValue);
        }
    }
}
