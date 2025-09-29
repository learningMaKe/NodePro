using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Attrs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeCategoryAttribute:Attribute
    {
        public string Category { get; set; }

        public NodeCategoryAttribute(string category)
        {
            Category = category;
        }
    }
}
