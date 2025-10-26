using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core
{
    public abstract class FactoryBase<TAttribute,TAssigned> where TAttribute : NodeRegisterAttribute where TAssigned:class
    {
        protected IProvider _provider;
        public abstract string Key { get; }

        private readonly Dictionary<string, TAssigned> _instanceMap = [];

        private readonly Dictionary<string, Type> _reflectionMap = [];

        protected event Action<TAssigned>? InstanceCreated;

        protected virtual string CreateKey(TAttribute attribute)
        {
            return attribute.GetExtraData() as string ?? string.Empty;
        }
        public FactoryBase(INodeRegister register, IProvider provider)
        {
            _provider = provider;
            Type[] types = register.GetRegisterTypes(Key);
            foreach (Type type in types)
            {
                TAttribute? lineAttribute = type.GetCustomAttribute<TAttribute>();
                if (lineAttribute is null) continue;
                string s = CreateKey(lineAttribute);
                if(string.IsNullOrEmpty(s)) continue;
                _reflectionMap.Add(s, type);
            }

        }

        /// <summary>
        /// 获取指定模式的线条计算器
        /// </summary>
        public TAssigned GetInstance(string mode)
        {
            // 检查缓存中是否已有实例
            if (_instanceMap.TryGetValue(mode, out var calculator))
            {
                return calculator;
            }

            // 创建新实例并缓存
            calculator = CreateInstance(mode);
            _instanceMap[mode] = calculator;
            return calculator;
        }

        protected virtual TAssigned CreateInstance(string mode)
        {
            if (!_reflectionMap.TryGetValue(mode, out Type? calculateType))
            {
                throw new InvalidOperationException($"{mode} map not found");
            }
            _provider.Register(RegisterType.Singleton, calculateType);
            TAssigned nodeLineCalculator =_provider.Resolve(calculateType) as TAssigned ?? throw new InvalidOperationException($"无法创建{calculateType.FullName}的无参实例");
            InstanceCreated?.Invoke(nodeLineCalculator);
            return nodeLineCalculator;
        }
    }
}
