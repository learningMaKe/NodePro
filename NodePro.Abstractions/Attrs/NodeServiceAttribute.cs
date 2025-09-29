using NodePro.Abstractions.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Attrs
{
    [AttributeUsage(AttributeTargets.Class)]
    [NodeRegisterFlag(NodeConstants.HandlerSingleton)]
    public class NodeServiceAttribute : NodeRegisterAttribute
    {
        public Type? From;
        public NodeServiceAttribute(Type? from = null) : base(NodeConstants.KeyServices)
        {
            From = from;
        }

        public override object? GetExtraData()
        {
            return From;
        }

    }
}
