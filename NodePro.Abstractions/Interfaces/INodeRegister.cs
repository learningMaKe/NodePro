using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Interfaces
{
    public interface INodeRegister
    {
        public NodeRegisteredData[] GetRegisteredData(NodeRegisterType nodeRegisterType);
        public NodeRegisteredData[] GetRegisteredData(string key);
        public Type[] GetRegisterTypes(string key);
        public Type[] GetRegisterTypes(NodeRegisterType type);
    }
}
