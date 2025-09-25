using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core
{
    public enum NodeExecuteResult
    {
        INVALID,
    }

    public class NodeExecutor
    {
        private readonly List<LinePair> _pairs = [];
        public NodeExecutor(List<LinePair> pairs)
        {
            _pairs = pairs;
        }

        public NodeExecuteResult Execute()
        {

            return NodeExecuteResult.INVALID;
        }
    }
}
