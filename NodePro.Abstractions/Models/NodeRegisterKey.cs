using NodePro.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Models
{
    public delegate bool NodeFilterHandler(NodeRegisterFilterParams paramters);

    public delegate void NodeSelectedHandler(Type selectedType, NodeRegisterParams parameters);

    public class NodeRegisterKey
    {

        public string Key { get; set; } = string.Empty;

        public NodeRegisterType RegisterType { get; set; }

        public NodeSelectedHandler? Selected { get; set; }
        public NodeFilterHandler? Filter { get; set; }

        public NodeRegisterKey()
        {

        }
    }

    public class CommonNodeRegisterKey:NodeRegisterKey
    {
        public CommonNodeRegisterKey(string key, NodeRegisterType type)
        {
            Key = key;
            RegisterType = type;
            Filter = x => x.Tag == key;
            Selected = (type, parameters) => { parameters.Add(key, type); };
        }
    }
}
