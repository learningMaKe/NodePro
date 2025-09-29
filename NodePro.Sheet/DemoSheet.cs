using NodePro.Abstractions.Attrs;
using NodePro.Core;

namespace NodePro.Sheet
{

    [Node,NodeCategory("Common")]
    public class DemoSheet : NodeSheet
    {
        public override string Title => "测试节点";

        [Input("NoFormat", "IntValue")]
        public string Value { get; set; } = "测试";


        [Input("NoFormat","StringValue"),NodeOrder(3)]
        public string Name { get; set; } = "你也要测试吗";


        [Input("NoFormat", "TestValue")]
        public string Test { get; set; } = "不想加班";

        [Output("Formateee"),NodeOrder(5)]
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
