using NodePro.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Exceptions
{
    public class NodeExecuteException:Exception
    {
        public NodeExecuteResult Result = NodeExecuteResult.INVALID;

        public NodeExecuteException(NodeExecuteResult result,string message):base(message)
        {
            Result = result;
        }
    }
}
