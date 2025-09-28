using NodePro.Abstractions.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Attrs
{
    [AttributeUsage(AttributeTargets.Class)]
    [NodeRegisterFlag(NodeRegisterConstants.HandlerSingleton)]
    public class NodeServiceAttribute : NodeRegisterAttribute
    {
        public Type? From;
        public NodeServiceAttribute(Type? from = null) : base(NodeRegisterConstants.Services)
        {
            From = from;
        }

        public override object? GetExtraData()
        {
            return From;
        }

    }
}
