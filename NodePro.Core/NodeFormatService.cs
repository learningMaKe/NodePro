using Microsoft.VisualBasic;
using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Interfaces;
using System;

namespace NodePro.Core
{
    [NodeService(typeof(INodeFormatService))]
    public class NodeFormatService :FactoryBase<NodeFormatAttribute,INodeFormatter> ,INodeFormatService
    {
        public NodeFormatService(INodeRegister register,IProvider provider):base(register,provider) 
        {

        }

        public override string Key => NodeConstants.KeyFormat;

        public object? Format(object source, string sourceType, string targetType)
        {
            string key = EncodeKey(sourceType, targetType);
            INodeFormatter formatter = GetInstance(key);
            if (formatter.CanFormat(source))
            {
                return formatter.FormatFrom(source);
            }
            return null;
        }

        public bool TryFormat(object source, string sourceType, out object? target, string targetType)
        {
            target = Format(source, sourceType, targetType);
            return target != null;
        }

        protected override string CreateKey(NodeFormatAttribute attribute)
        {
            return EncodeKey(attribute.Source, attribute.Target);
        }

        private string EncodeKey(string source,string target)
        {
            return $"{source}==!!=={target}";
        }

    }
}
