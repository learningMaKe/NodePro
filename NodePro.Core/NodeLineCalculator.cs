using NodePro.Core.Attrs;
using NodePro.Core.Lines;
using NodePro.Core.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NodePro.Core
{
    public static class LineCalculatorFactory
    {
        private static readonly Dictionary<LineCalculateMode, INodeLineCalculator> _instanceMap = [];
        // 反射映射表，缓存枚举值与计算器类型的对应关系
        private static readonly Dictionary<LineCalculateMode, Type> _reflectionMap = [];

        static LineCalculatorFactory()
        {
            // 初始化反射映射表
            InitializeReflectionMap();
        }

        /// <summary>
        /// 获取指定模式的线条计算器
        /// </summary>
        public static INodeLineCalculator GetCalculator(LineCalculateMode mode)
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

        public static INodeLineCalculator CreateCalculator(LineCalculateMode mode)
        {
            if (!_reflectionMap.TryGetValue(mode, out Type? calculateType))
            {
                throw new InvalidOperationException($"{mode} map not found");
            }
            INodeLineCalculator nodeLineCalculator = Activator.CreateInstance(calculateType) as INodeLineCalculator ?? throw new InvalidOperationException($"无法创建{calculateType.FullName}的无参实例");
            return nodeLineCalculator;
        }
        /// <summary>
        /// 初始化反射映射表，通过反射获取枚举值与计算器类型的对应关系
        /// </summary>
        private static void InitializeReflectionMap()
        {
            var enumType = typeof(LineCalculateMode);
            var enumValues = Enum.GetValues(enumType);

            foreach (LineCalculateMode mode in enumValues)
            {
                var memberInfo = enumType.GetMember(mode.ToString())[0];

                if (memberInfo.GetCustomAttributes(typeof(NodeLineAttribute), false)
                                          .FirstOrDefault() is not NodeLineAttribute attribute) continue;
                _reflectionMap.TryAdd(mode, attribute.CalculatorType);

            }
        }
    }

    public enum LineCalculateMode
    {
        [NodeLine(typeof(StraightLine))]
        Straight,

        [NodeLine(typeof(CurveLine))]
        Curved,
        // [NodeLine(typeof(AngledLineCalculator))]
        // Angled
    }

    public interface INodeLineCalculator
    {
        PathFigure Calculate(Point start, Point end);
    }

}
