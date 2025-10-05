using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Models
{
    public class NodeRegisterKey
    {

        public string Key { get; set; } = string.Empty;

        public IScanBehavior? ScanBehavior { get; set; }

        public NodeRegisterKey()
        {

        }
    }

}
