using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Interfaces
{
    public interface INodeExecutor
    {
        public NodeExecuteResult Execute(INodeContainer[] nodes, List<LinePair> pairs);
    }
}
