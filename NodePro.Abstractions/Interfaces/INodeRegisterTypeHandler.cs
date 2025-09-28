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
    /// <summary>
    /// 专门为 枚举类<see cref="NodeRegisterType"/> 设计，通过<see cref="NodeRegisterFlagAttribute"/>为字段打上标记，定义字段的具体注册方式 
    /// </summary>
    public interface INodeRegisterTypeHandler
    {
        public void OnRegister(NodeRegisteredData data, IContainerRegistry containerRegistry);
    }
}
