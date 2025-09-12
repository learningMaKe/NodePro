using NodePro.Core.Attrs;
using NodePro.Core.Interfaces;
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
        private LineCalculateMode _lineMode = LineCalculateMode.Straight;
        public LineCalculateMode LineMode
        {
            get => _lineMode;
            set => _lineMode = value;
        }
        public NodeLine CreateLine(INotifyPosition source,INotifyPosition target)
        {
            NodeLine line = new NodeLine(source, target)
            {
                Mode = LineMode
            };
            return line;
        }

        public NodeLine CreateLine(NodeConnectEventArgs args) => CreateLine(args.NodeSource.Connector, args.NodeTarget.Connector);
    }
}
