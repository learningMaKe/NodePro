using NodePro.Core.Attrs;
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
                throw new InvalidOperationException($"{mode} map not fou");
            }
            INodeLineCalculator nodeLineCalculator = Activator.CreateInstance(calculateType) as INodeLineCalculator ?? throw new InvalidOperationException($"无法创建{calculateType.FullName}的无参示例");
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
                var attribute = memberInfo.GetCustomAttributes(typeof(NodeLineAttribute), false)
                                          .FirstOrDefault() as NodeLineAttribute;

                if (attribute is null) continue;
                _reflectionMap.TryAdd(mode, attribute.CalculatorType);

            }
        }
    }

    public enum LineCalculateMode
    {
        [NodeLine(typeof(StraightLineCalculator))]
        Straight,

        // 可以在这里添加更多线条类型
        // [NodeLine(typeof(CurvedLineCalculator))]
        // Curved,
        // 
        // [NodeLine(typeof(AngledLineCalculator))]
        // Angled
    }

    public interface INodeLineCalculator
    {
        PathFigure Calculate(Point start, Point end);
    }

    public class StraightLineCalculator : INodeLineCalculator
    {
        public PathFigure Calculate(Point start, Point end)
        {
            // 创建直线图形
            var figure = new PathFigure
            {
                StartPoint = start,
                IsClosed = false,
                IsFilled = false
            };

            // 添加直线段
            figure.Segments.Add(new LineSegment(end, true));

            return figure;
        }
    }
}
