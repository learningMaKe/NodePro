using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace NodePro.Abstractions.Exceptions
{
    public class NodeMissingException:Exception
    {
        public Type MissingType { get; private set; }
        

        public NodeMissingException(string msg,Type missingType):base(msg) 
        {
            MissingType = missingType;
        }

        public override string ToString()
        {
            return $"{Message}:{MissingType.FullName}";
        }

        [DoesNotReturn]
        public static void Throw<TSheet>() where TSheet : INodeSheet
        {
            throw new NodeMissingException($"缺失节点:{typeof(TSheet).Name}", typeof(TSheet));
        }
    }
}
