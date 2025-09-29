using NodePro.Abstractions.Attrs;
using NodePro.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Sheet
{
    [Node,NodeCategory("Demo2")]
    public class Demo2Sheet:NodeSheet
    {
        public override string Title => "测试节点";

        [Input("NoFormat", "IntValue")]
        public string Value { get; set; } = "测试";


        [Input("NoFormat", "StringValue"), NodeOrder(3)]
        public string Name { get; set; } = "你也要测试吗";


        [Input("NoFormat", "TestValue")]
        public string Test { get; set; } = "不想加班";

        [Output("Formateee"), NodeOrder(5)]
        public string Demo { get; set; } = "阿西吧";

        public DemoSheet(IContainerProvider provider) : base(provider)
        {

        }

        public override NodeData DataProcess(NodeData data)
        {
            throw new NotImplementedException();
        }
    }
}
