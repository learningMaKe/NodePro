using NodePro.Abstractions.Enums;
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
            Type[] singletons = register.GetRegisterTypes(NodeRegisterType.Singleton);
            Type[] instances = register.GetRegisterTypes(NodeRegisterType.Instance);
            foreach (var singleton in singletons)
            {
                containerRegistry.RegisterSingleton(singleton);
            }
            foreach(var instance in instances)
            {
                containerRegistry.Register(instance);
            }
        }
    }
}
