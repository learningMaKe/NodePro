using NodePro.Abstractions.Interfaces;
using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Registers
{
    public class InstanceHandler : INodeRegisterTypeHandler
    {
        public void OnRegister(NodeRegisteredData data, IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(data.DataType);
        }
    }
}
