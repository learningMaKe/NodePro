using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Models;

namespace NodePro.Abstractions.Attrs
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NodePropertyAttribute : Attribute
    {
        /// <summary>
        /// 用于定义数据格式，在数据传输时会检查格式是否能成功转化
        /// </summary>
        public string Format { get; } = string.Empty;
        /// <summary>
        /// 用于定义显示在UI上的模板
        /// </summary>
        public string Template { get; } = string.Empty;
        public NodeMode Mode { get; } = NodeMode.Input;
        public NodePropertyAttribute(string format, string template,NodeMode mode)
        {
            Format = format;
            Template = template;
            Mode = mode;
        }

    }
    
    public class InputAttribute : NodePropertyAttribute
    {
        public InputAttribute(string format, string template) : base(format, template,NodeMode.Input)
        {

        }
    }

    public class OutputAttribute : NodePropertyAttribute
    {
        public OutputAttribute(string format) : base(format, NodeTemplateKey.DefaultOutputTemplate, NodeMode.Output)
        {

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
