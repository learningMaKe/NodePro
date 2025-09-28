using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Models
{
    public class NodeRegisteredData
    {
        public required Type DataType { get; init; }

        public object? ExtraData { get; init; }

        public INodeRegisterTypeHandler? Handler { get; init; }
    }
}
