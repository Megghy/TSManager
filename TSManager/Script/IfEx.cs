using ScratchNet;

namespace TSManager.Script
{
    class IfEx : Statement
    {
        public IfEx() => Consequent = new BlockStatement();
        public Expression GiveValue { get; set; }
        public BlockStatement Consequent
        {
            get;
            set;
        }
        public BlockStatement Alternate { get; set; }

        public override string ReturnType => "void";
        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            if (GiveValue == null)
                return Completion.Exception("所给值无效", this);
            var t = GiveValue.Execute(enviroment);
            if (t.Type != CompletionType.Value)
                return t;
            if (t.ReturnValue is bool result)
            {
                if (result)
                {
                    if (Consequent == null)
                        return Completion.Void;
                    ExecutionEnvironment current = new(enviroment);
                    return Consequent.Execute(current);
                }
                else
                {
                    if (Alternate == null)
                        return Completion.Void;
                    ExecutionEnvironment current = new(enviroment);
                    return Alternate.Execute(current);
                }
            }
            else return Completion.Exception("所给的值非bool值", GiveValue);
        }

        public override Descriptor Descriptor => new()
        {
            new TextItemDescriptor(this, "如果 "),
            new ExpressionDescriptor(this, "GiveValue", "boolean|bool")
        };
        public override BlockDescriptor BlockDescriptor
        {
            get
            {
                BlockDescriptor block = new();
                block.Add(new BlockStatementDescriptor(this, "Consequent"));
                if (Alternate != null)
                {
                    Descriptor d = new()
                    {
                        new TextItemDescriptor(this, "否则"),
                    };
                    block.Add(new ExpressionStatementDescription(this, "else", d));
                    block.Add(new BlockStatementDescriptor(this, "Alternate"));
                }
                return block;
            }
        }
        public override string Type => "IfStatement";
        public override bool IsClosing => false;
    }
}
