using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Models
{
    public delegate void NodeSelectedHandler(Type selectedType, NodeRegisterParameters parameters);

    public class NodeRegisterKey
    {

        public required string Key { get; set; }

        public NodeSelectedHandler? Selected { get; set; }
        public required Func<Type, bool> Filter { get; set; }

        public NodeRegisterKey()
        {

        }
    }
}
