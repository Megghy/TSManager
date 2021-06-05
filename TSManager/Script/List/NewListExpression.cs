using System.Collections.Generic;

namespace ScratchNet
{
    public class NewListExpression : Expression
    {
        public override string ReturnType => "list";

        public override Descriptor Descriptor => new()
        {
            new TextItemDescriptor(this, "创建新列表", true),
        };

        public override string Type => "NewListExpression";

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            return new Completion(new List<object>());
        }
    }
}
