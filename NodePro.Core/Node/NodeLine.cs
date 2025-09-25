using NodePro.Abstractions;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NodePro.Core.Node
{
    public class NodeLine:NodeLineBase
    {
        private LineCalculateMode _mode = LineCalculateMode.Straight;
        private INodeLineCalculator? _lineCalculator;

        public INotifyPosition? Source {  get; private set; }

        public INotifyPosition? Target { get; private set; }

        public LineCalculateMode Mode
        {
            get => _mode;
            set
            {
                if (_mode == value) return;
                _mode = value;
                OnModeChanged();
            }
        }


        public PathGeometry Data
        {
            get { return (PathGeometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(PathGeometry), typeof(NodeLine), new PropertyMetadata(null));

        public NodeLine(INotifyPosition source,INotifyPosition target)
        {
            Source = source;
            Target = target;
            _lineCalculator = LineCalculatorFactory.GetCalculator(_mode);
            Data = new PathGeometry();
            Canvas.SetZIndex(this, -100);
            WeakEventManager<INotifyPosition, PositionChangedEventArgs>.AddHandler(Source, nameof(INotifyPosition.PositionChangedEventHandler), OnNodeMove);
            WeakEventManager<INotifyPosition, PositionChangedEventArgs>.AddHandler(Target, nameof(INotifyPosition.PositionChangedEventHandler), OnNodeMove);
            Recalculate();
        }

        private void OnNodeMove(object? notifier, PositionChangedEventArgs args)
        {
            Recalculate();
        }

        private void OnModeChanged()
        {
            _lineCalculator = LineCalculatorFactory.GetCalculator(_mode);
            Recalculate();
        }

        private void Recalculate()
        {
            Data.Clear();
            if (_lineCalculator is null) return;
            PathFigure figure = _lineCalculator.Calculate(Source!.Position, Target!.Position);
            Data.Figures.Add(figure);

        }
    }
}
