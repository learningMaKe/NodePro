using NodePro.Abstractions.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Attrs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeFormatAttribute:NodeRegisterAttribute
    {
        public string Source;
        public string Target;
        public NodeFormatAttribute(string source,string target):base(NodeConstants.KeyFormat,NodeConstants.HandlerSingleton)
        {
            Source= source;
            Target= target;
        }

    }
}
