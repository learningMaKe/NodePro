using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Formats
{
    [NodeFormat("Demo1","Demo2")]
    public class DemoFormatter : INodeFormatter
    {
        public bool CanFormat(object source)
        {
            return true;
        }

        public object FormatFrom(object source)
        {
            if(source is string s)
            {
                return s + "World";
            }
            return source;
        }
    }
}
