using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Attrs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeRegisterAttribute:Attribute
    {
        public string Tag { get;private set; }
        public NodeRegisterAttribute(string tag)
        {
            Tag = tag;
        }

    }
}
