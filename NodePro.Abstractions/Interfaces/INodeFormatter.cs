using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Interfaces
{
    public interface INodeFormatter
    {
        public bool CanFormat(object source);
        public object FormatFrom(object source);
    }
}
