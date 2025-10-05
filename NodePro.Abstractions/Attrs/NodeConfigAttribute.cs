using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Attrs
{
    [AttributeUsage(AttributeTargets.Assembly,AllowMultiple =true)]
    public class NodeConfigAttribute:Attribute
    {
        public string Path;
        public NodeConfigAttribute(string path)
        {
            Path = path;
        }
    }
}
