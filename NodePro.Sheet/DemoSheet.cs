using NodePro.Core;
using NodePro.Core.Attrs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Sheet
{

    [Node]
    public class DemoSheet : NodeSheet
    {
        public override string Title => "测试节点";

        [NodeProperty("NoFormat", "IntValue")]
        public string Value { get; set; } = "测试";


        [NodeProperty("NoFormat","StringValue")]
        public string Name { get; set; } = "你也要测试吗";


        [NodeProperty("NoFormat", "TestValue")]
        public string Test { get; set; } = "不想加班";

        public DemoSheet(IContainerProvider provider) : base(provider)
        {

        }

        public override NodeData DataProcess(NodeData data)
        {
            throw new NotImplementedException();
        }
    }
}
