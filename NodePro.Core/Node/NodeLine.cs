using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NodePro.Core.Node
{
    public class NodeLine:FrameworkElement
    {


        public PathGeometry Data
        {
            get { return (PathGeometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(PathGeometry), typeof(NodeLine), new PropertyMetadata(null));



        public LineCalculateMode CalculateMode
        {
            get { return (LineCalculateMode)GetValue(CalculateModeProperty); }
            set { SetValue(CalculateModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CalculateMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CalculateModeProperty =
            DependencyProperty.Register("CalculateMode", typeof(LineCalculateMode), typeof(NodeLine), new PropertyMetadata(LineCalculateMode.Straight,OnCalculateModeChanged));

        private static void OnCalculateModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is NodeLine line)
            {
                line.Calculator = LineCalculatorFactory.GetCalculator((LineCalculateMode)e.NewValue);
            }
        }

        private INodeLineCalculator? Calculator { get; set; } = null;

        public NodeLine()
        {
                
        }


    }
}
