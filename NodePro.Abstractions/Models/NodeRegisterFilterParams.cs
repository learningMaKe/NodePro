using NodePro.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Models
{
    public class NodeRegisterFilterParams
    {
        public required Type TypeToFilter;

        public string Tag = string.Empty;


    }
}
