using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Attrs
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NodePropertyAttribute:Attribute
    {
        public string Format { get; }=string.Empty;
        public string Template { get; } = string.Empty;
        public NodePropertyAttribute(string format, string template)
        {
            Format = format;
            Template = template;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NodeOrderAttribute(int order) : Attribute
    {
        public int Order { get; } = order;
    }

    
}
