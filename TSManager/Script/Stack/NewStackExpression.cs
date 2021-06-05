using System.Collections.Generic;

namespace ScratchNet
{
    public class NewStackExpression : Expression
    {
        public override string ReturnType => "stack";

        public override Descriptor Descriptor
        {
            get
            {
                Descriptor desc = new Descriptor();
                desc.Add(new TextItemDescriptor(this, "new", true));
                desc.Add(new TextItemDescriptor(this, " Stack()"));
                return desc;
            }
        }

        public override string Type => "NewStackExpression";

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            return new Completion(new Stack<object>());
        }
    }
}
