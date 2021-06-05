using System;
using System.Collections.Generic;

namespace ScratchNet
{
    public class ListRemoveAtExpression : Expression
    {
        public Expression List { get; set; }
        public Expression Index { get; set; }
        public override string ReturnType => "void";

        public override Descriptor Descriptor => new()
        {
            new TextItemDescriptor(this, "移除列表 [ "),
            new ExpressionDescriptor(this, "List", "any") { NothingAllowed = true },
            new TextItemDescriptor(this, "] 第 [ "),
            new ExpressionDescriptor(this, "Index", "number") { IsOnlyNumberAllowed = true },
            new TextItemDescriptor(this, "] 处的成员"),
        };

        public override string Type => "ListInsertExpression";

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            if (List == null || Index == null)
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
            int id = (int)i.ReturnValue;

            List<object> stack = c.ReturnValue as List<object>;
            if (id < 0 || id >= stack.Count)
            {
                return Completion.Exception("位数超出列表长度", Index);
            }
            try
            {
                object ov = stack[id];
                stack.RemoveAt(id);
                return new Completion(ov);
            }
            catch (Exception e)
            {
                return Completion.Exception(e.Message, Index);
            }
        }
    }
}
