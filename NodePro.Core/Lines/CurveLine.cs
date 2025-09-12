using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NodePro.Core.Lines
{
    public class CurveLine : INodeLineCalculator
    {
        // 曲线的垂直偏移量，控制弯曲程度（可根据需要调整）
        private const double CurveOffset = 50;

        public PathFigure Calculate(Point start, Point end)
        {
            // 创建曲线图形
            var figure = new PathFigure
            {
                StartPoint = start,
                IsClosed = false,
                IsFilled = false
            };

            double mid = (start.X + end.X) * 0.5;
            Point low = new Point(mid, start.Y);
            Point high = new Point(mid, end.Y);
            var curve = new BezierSegment()
            {
                Point1 = low,
                Point2 = high,
                Point3 = end,
            };
            figure.Segments.Add(curve);
            return figure;
        }

    }
}
