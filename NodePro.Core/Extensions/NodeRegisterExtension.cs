using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Extensions
{
    public static class NodeRegisterExtension
    {
        public static void PrismIoc(this NodeRegister register,IContainerRegistry containerRegistry)
        {
            NodeRegisterType[] types = Enum.GetValues<NodeRegisterType>();
            foreach (var type in types)
            {
                NodeRegisteredData[] registedDatas = register.GetRegisteredData(type);
                foreach (var data in registedDatas)
                {
                    if (data.Handler is null) continue;
                    data.Handler.OnRegister(data, containerRegistry);
                }
            }
        }
    }
}
