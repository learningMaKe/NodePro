using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Attrs
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NodeLineAttribute:Attribute
    {
        public string Key = string.Empty;

        public NodeLineAttribute(string key)
        {
            Key = key;
        }
    }
}
