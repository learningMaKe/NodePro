using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NodePro.Abstractions.Interfaces
{

    public interface INodeLineCalculator
    {
        PathFigure Calculate(Point start, Point end);
    }
}
