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
        public string Template { get; } = string.Empty;
        public Type? Conveter { get; } = null;
        public NodePropertyAttribute(string template, Type? converter = null)
        {
            Template = template;
            Conveter = converter;
        }
    }
}
