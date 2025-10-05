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
    public class InstanceRegisterBehavior : IRegisterBehavior
    {
        public void OnRegister(NodeRegisteredData data, IProvider containerRegistry)
        {
            containerRegistry.Register(RegisterType.Normal, data.DataType);

        }
    }
}
