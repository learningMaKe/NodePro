using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Interfaces
{
    public interface INodeFormatService
    {
        public bool TryFormat(object source, string sourceType, out object? target, string targetType);

        public object? Format(object source, string sourceType, string targetType);
    }
}
