using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Models;

namespace NodePro.Abstractions.Interfaces
{
    public interface IRegisterBehavior
    {
        public void OnRegister(NodeRegisteredData data, IProvider containerRegistry);
    }
}
