using NodePro.Abstractions.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Attrs
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class NodeAttribute : NodeRegisterAttribute
    {
        public NodeAttribute() : base(NodeConstants.KeyNodes, NodeConstants.HandlerInstance)
        {
        }
    }
}
