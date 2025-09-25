using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Enums;

namespace NodePro.Abstractions.Attrs
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
            Template = NodeTemplateKey.DefaultOutputTemplate;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NodeOrderAttribute : Attribute
    {
        public int Order { get; }

        public NodeOrderAttribute(int order)
        {
            Order = order;
        }
    }


}
