using NodePro.Abstractions.Arguments;
using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Interfaces;
using NodePro.Core.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core
{
    [NodeService]
    public class NodeLineCreator
    {
        private string _lineMode = NodeLineConstants.Straight;
        private readonly LineCalculatorFactory _lineCalculatorFactory;
        public string LineMode
        {
            get => _lineMode;
            set => _lineMode = value;
        }

        public NodeLineCreator(LineCalculatorFactory factory)
        {
            _lineCalculatorFactory = factory;
        }

        public NodeLine CreateLine(INotifyPosition source,INotifyPosition target)
        {
            NodeLine line = new NodeLine(source, target, _lineCalculatorFactory)
            {
                Mode = LineMode,
            };
            return line;
        }

        public NodeLine CreateLine(NodeConnectEventArgs args) => CreateLine(args.NodeSource.Connector, args.NodeTarget.Connector);
    }
}
