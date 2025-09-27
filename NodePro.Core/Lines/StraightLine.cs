using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NodePro.Core.Lines
{
    [NodeLine(NodeLineConstants.Straight)]
    public class StraightLine : INodeLineCalculator
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
