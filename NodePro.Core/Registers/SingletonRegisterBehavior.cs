using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Interfaces;
using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Registers
{
    public class SingletonRegisterBehavior : IRegisterBehavior
    {
        public void OnRegister(NodeRegisteredData data, IProvider containerRegistry)
        {
            Type? interfaceType = data.ExtraData as Type;
            if (interfaceType != null && data.DataType.IsAssignableTo(interfaceType))
            {
                containerRegistry.Register(RegisterType.Singleton, interfaceType, data.DataType);
            }
            else
            {
                containerRegistry.Register(RegisterType.Singleton, data.DataType);
            }
        }
    }
}
