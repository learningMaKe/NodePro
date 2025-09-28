using NodePro.Abstractions.Interfaces;
using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Registers
{
    public class SingletonHandler : INodeRegisterTypeHandler
    {
        public void OnRegister(NodeRegisteredData data, IContainerRegistry containerRegistry)
        {
            Type? interfaceType = data.ExtraData as Type;
            if (interfaceType != null&&data.DataType.IsAssignableFrom(interfaceType)) 
            {
                containerRegistry.Register(interfaceType, data.DataType);
            }
            else
            {
                containerRegistry.Register(data.DataType);
            }
        }
    }
}
