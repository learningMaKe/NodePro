using NodePro.Abstractions.Interfaces;
using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Registers
{
    public class CommonScanBehavior : IScanBehavior
    {
        public bool Filter(NodeRegisterKey key, NodeRegisterFilterParams paramters)
        {
            return paramters.Tag == key.Key;
        }

        public void Selected(NodeRegisterKey key, NodeRegisteredData data, NodeRegisterParams parameters)
        {
            parameters.Add(key.Key, data);
        }
    }
}
