using NodePro.Core.Model;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Attrs
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NodePropertyAttribute : Attribute
    {
        public string Format { get; } = string.Empty;
        public string Template { get; } = string.Empty;
        public NodeMode Mode { get; } = NodeMode.Input;
        public NodePropertyAttribute(string format, string template)
        {
            Mode = NodeMode.Input;
            Format = format;
            Template = template;
        }

        public NodePropertyAttribute(string format)
        {
            Mode = NodeMode.Output;
            Format = format;
            Template = TemplateKey.DefaultNodeTemplate;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NodeOrderAttribute(int order) : Attribute
    {
        public int Order { get; } = order;
    }


}
