using System.Collections.Generic;

namespace ScratchNet
{
    public class ListValueExpression : Expression, IAssignment
    {
        public Expression List { get; set; }
        public Expression Index { get; set; }
        public override string ReturnType => "any";

        public override Descriptor Descriptor => new()
        {
            new TextItemDescriptor(this, "取列表 ["),
            new ExpressionDescriptor(this, "List", "any") { NothingAllowed = true },
            new TextItemDescriptor(this, "] 的第 ["),
            new ExpressionDescriptor(this, "Index", "number"),
            new TextItemDescriptor(this, "] 位")
        };

        public override string Type => "ListValueExpression";

        public Completion Assign(ExecutionEnvironment enviroment, object value)
        {
            if (List == null || Index == null)
                return Completion.Exception("无效的数组", this);
            var a = List.Execute(enviroment);
            if (!a.IsValue)
                return a;
            if (a.ReturnValue is Dictionary<object, object>)
            {
                var i = Index.Execute(enviroment);
                if (!i.IsValue)
                    return i;
                if ((i.ReturnValue == null))
                    return Completion.Exception("无效的键值", Index);
                var iv = i.ReturnValue;
                Dictionary<object, object> d = a.ReturnValue as Dictionary<object, object>;
                d[iv] = value;
                return new Completion(value);
            }
            else
            {
                var i = Index.Execute(enviroment);
                if (!i.IsValue)
                    return i;
                if (!(i.ReturnValue is int))
                    return Completion.Exception("位数仅允许填写int类型数据", Index);
                var iv = (int)i.ReturnValue;
                if (a.ReturnValue is object[])
                {
                    object[] arra = a.ReturnValue as object[];
                    if (iv < 0 && iv >= arra.Length)
                    {
                        return Completion.Exception(iv + " 超出数组长度", Index);
                    }
                    arra[iv] = value;
                    return new Completion(value);
                }
                else if (a.ReturnValue is List<object>)
                {
                    List<object> arra = a.ReturnValue as List<object>;
                    if (iv < 0 && iv >= arra.Count)
                    {
                        return Completion.Exception(iv + " 超出列表长度", Index);
                    }
                    arra[iv] = value;
                    return new Completion(value);
                }
                return Completion.Exception("数据不是数组. 列表. 或者字典", List);
            }
        }

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            if (List == null || Index == null)
                return Completion.Exception("Null Exception", this);
            var a = List.Execute(enviroment);
            if (!a.IsValue)
                return a;
            if (a.ReturnValue is Dictionary<object, object>)
            {
                var i = Index.Execute(enviroment);
                if (!i.IsValue)
                    return i;
                if ((i.ReturnValue == null))
                    return Completion.Exception("dictionary key can not be null", Index);
                var iv = i.ReturnValue;
                Dictionary<object, object> d = a.ReturnValue as Dictionary<object, object>;
                if (d.ContainsKey(iv))
                    return new Completion(d[iv]);
                return Completion.Void; ;
            }
            else
            {
                var i = Index.Execute(enviroment);
                if (!i.IsValue)
                    return i;
                if (!(i.ReturnValue is int))
                    return Completion.Exception("Only integer is accepted", Index);
                var iv = (int)i.ReturnValue;
                if (a.ReturnValue is object[])
                {
                    object[] arra = a.ReturnValue as object[];
                    if (iv < 0 && iv >= arra.Length)
                    {
                        return Completion.Exception("value " + iv + " is out of array index", Index);
                    }
                    return new Completion(arra[iv]);
                }
                else if (a.ReturnValue is List<object>)
                {
                    List<object> arra = a.ReturnValue as List<object>;
                    if (iv < 0 && iv >= arra.Count)
                    {
                        return Completion.Exception("value " + iv + " is out of list index", Index);
                    }
                    return new Completion(arra[iv]);
                }
                return Completion.Exception("value is not an array, list or dictionary", List);
            }
        }
    }
}
