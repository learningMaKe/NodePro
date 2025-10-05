using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Interfaces
{
    public delegate bool NodeFilterHandler(NodeRegisterFilterParams paramters);

    public delegate void NodeSelectedHandler(NodeRegisteredData data, NodeRegisterParams parameters);

    public interface IScanBehavior
    {
        public bool Filter(NodeRegisterKey key, NodeRegisterFilterParams paramters);
        public void Selected(NodeRegisterKey key, NodeRegisteredData data, NodeRegisterParams parameters);

    }
}
