using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Formats
{
    [NodeFormat(NodeFormats.Int,NodeFormats.String)]
    public class IntStringFormatter : INodeFormatter
    {
        public bool CanFormat(object source)
        {
            return source is int;
        }

        public object FormatFrom(object source)
        {
            int value = (int)source;
            return value.ToString();
        }
    }
}
