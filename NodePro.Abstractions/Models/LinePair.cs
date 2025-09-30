using NodePro.Abstractions.Arguments;
using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Models
{
    public struct LinePair
    {
        public ConnectionEndpoint Source { get; set; }

        public ConnectionEndpoint Target { get; set; }

        public INodeLine Line { get; set; }

        public LinePair(ConnectionEndpoint source, ConnectionEndpoint target, INodeLine line)
        {
            Source = source;
            Target = target;
            Line = line;
        }

        public LinePair(NodeConnectEventArgs args, INodeLine line)
        {
            Source = args.NodeSource;
            Target = args.NodeTarget;
            Line = line;
        }
    }
}
