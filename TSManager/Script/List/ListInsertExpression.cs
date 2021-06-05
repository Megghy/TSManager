using System;
using System.Collections.Generic;

namespace ScratchNet
{
    public class ListInsertExpression : Expression
    {
        public Expression List { get; set; }
        public Expression Value { get; set; } = new Literal("value");
        public Expression Index { get; set; } = new Literal("index");
        public override string ReturnType => "any";

        public override Descriptor Descriptor => new()
        {
            new TextItemDescriptor(this, "在列表 ["),
            new ExpressionDescriptor(this, "List", "any") { NothingAllowed = true },
            new TextItemDescriptor(this, "] 的第 ["),
            new ExpressionDescriptor(this, "Index", "number") { IsOnlyNumberAllowed = true },
            new TextItemDescriptor(this, "] 处加入成员 ["),
            new ExpressionDescriptor(this, "Value", "any"),
            new TextItemDescriptor(this, "]"),
        };

        public override string Type => "ListInsertExpression";

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            if (List == null || Value == null || Index == null)
                return Completion.Exception("无效的数组", this);
            Completion c = List.Execute(enviroment);
            if (!c.IsValue)
                return c;
            if (!(c.ReturnValue is List<object>))
                return Completion.Exception("此处仅允许列表类型的数据", List);
            Completion i = Index.Execute(enviroment);
            if (!i.IsValue)
                return i;
            if (!(i.ReturnValue is int))
                return Completion.Exception("列表位数仅允许填写int类型数据", Index);
            Completion v = Value.Execute(enviroment);
            if (!v.IsValue)
                return v;
            List<object> stack = c.ReturnValue as List<object>;
            try
            {
                stack.Insert((int)i.ReturnValue, v.ReturnValue);
            }
            catch (Exception e)
            {
                return Completion.Exception(e.Message, Index);
            }
            return new Completion(v.ReturnValue);
        }
    }
}
