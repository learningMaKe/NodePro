using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Enums
{
    
    public enum NodeExecuteResult
    {
        SUCCESS,
        NO_NODE,
        MISSING_INPUT,
        MISSING_OUTPUT,
        LOOP_DETECTED,
        INVALID,
    }
}
