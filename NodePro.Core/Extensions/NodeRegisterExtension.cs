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
            NodeRegisteredData[] singletons = register.GetRegisteredData(NodeRegisterType.Singleton);
            Type[] instances = register.GetRegisterTypes(NodeRegisterType.Instance);

            foreach (var singleton in singletons)
            {
                Type? interfaceType = singleton.ExtraData as Type;
                if (interfaceType!=null &&singleton.DataType.IsAssignableFrom(interfaceType))
                {
                    containerRegistry.RegisterSingleton(interfaceType, singleton.DataType);
                }
                else
                {
                    containerRegistry.RegisterSingleton(singleton.DataType);
                }
            }
            foreach(var instance in instances)
            {
                containerRegistry.Register(instance);
            }
        }
    }
}
