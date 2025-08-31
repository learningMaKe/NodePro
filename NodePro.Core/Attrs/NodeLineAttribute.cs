using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Attrs
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NodeLineAttribute:Attribute
    {
        public Type CalculatorType { get; set; }
        public NodeLineAttribute(Type calculateType)
        {
            CalculatorType = calculateType;
        }
    }
}
