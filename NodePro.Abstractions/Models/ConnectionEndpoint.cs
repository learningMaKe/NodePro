using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Models
{
    // 代表连接的一方（节点+连接器）
    public readonly struct ConnectionEndpoint(INodeContainer node, INodeConnector connector)
    {
        public INodeContainer Node { get; } = node ?? throw new ArgumentNullException(nameof(node));
        public INodeConnector Connector { get; } = connector ?? throw new ArgumentNullException(nameof(connector));
    }
}
