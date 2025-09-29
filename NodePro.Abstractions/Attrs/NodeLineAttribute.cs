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
    public class NodeLineAttribute:NodeRegisterAttribute
    {
        public string Key = string.Empty;

        public NodeLineAttribute(string key):base(NodeConstants.KeyLines)
        {
            Key = key;
        }
    }
}
