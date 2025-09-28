using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Attrs
{

    [AttributeUsage(AttributeTargets.Class)]
    public class NodeRegisterFlagAttribute : Attribute
    {
        public string Handler { get; init; }
        public NodeRegisterFlagAttribute(string handler)
        {
            Handler = handler;
        }
    }
}
