using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Interfaces;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace NodePro.Core
{
    [NodeService(typeof(ILineCalculatorFactory))]
    public class LineCalculatorFactory:ILineCalculatorFactory
    {
        private readonly Dictionary<string, INodeLineCalculator> _instanceMap = [];
        // 反射映射表，缓存枚举值与计算器类型的对应关系
        private readonly Dictionary<string, Type> _reflectionMap = [];

        public LineCalculatorFactory(INodeRegister register)
        {
            Type[] types = register.GetRegisterTypes(NodeConstants.KeyLines);
            foreach (Type type in types) 
            {
                NodeLineAttribute? lineAttribute = type.GetCustomAttribute<NodeLineAttribute>();
                if (lineAttribute is null) continue;
                _reflectionMap.Add(lineAttribute.Key, type);
            }
        }

        /// <summary>
        /// 获取指定模式的线条计算器
        /// </summary>
        public INodeLineCalculator GetCalculator(string mode)
        {
            // 检查缓存中是否已有实例
            if (_instanceMap.TryGetValue(mode, out var calculator))
            {
                return calculator;
            }

            // 创建新实例并缓存
            calculator = CreateCalculator(mode);
            _instanceMap[mode] = calculator;
            return calculator;
        }

        private INodeLineCalculator CreateCalculator(string mode)
        {
            if (!_reflectionMap.TryGetValue(mode, out Type? calculateType))
            {
                throw new InvalidOperationException($"{mode} map not found");
            }
            INodeLineCalculator nodeLineCalculator = Activator.CreateInstance(calculateType) as INodeLineCalculator ?? throw new InvalidOperationException($"无法创建{calculateType.FullName}的无参实例");
            return nodeLineCalculator;
        }
    }

    


}
