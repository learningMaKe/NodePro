using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Registers
{
    public class PrismProvider : IProvider
    {
        private readonly IContainerProvider _containerProvider;
        private readonly IContainerRegistry _containerRegistry;
        public PrismProvider(IContainerProvider containerProvider, IContainerRegistry containerRegistry)
        {
            _containerProvider = containerProvider;
            _containerRegistry = containerRegistry;
        }

        public void Register(RegisterType type, Type target)
        {
            switch (type)
            {
                case RegisterType.Normal:_containerRegistry.Register(target); break;
                case RegisterType.Singleton:_containerRegistry.RegisterSingleton(target); break;
            }
        }

        public void Register(RegisterType type, Type from, Type to)
        {
            switch (type)
            {
                case RegisterType.Normal: _containerRegistry.Register(from,to); break;
                case RegisterType.Singleton:_containerRegistry.RegisterSingleton(from,to); break;
            }
        }

        public T Resolve<T>()
        {
            return _containerProvider.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return _containerProvider.Resolve(type);
        }
    }
}
