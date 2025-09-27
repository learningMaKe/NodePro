using NodePro.Abstractions;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NodePro.Core.Node
{
    public class NodeLine:NodeLineBase
    {
        private string _mode = NodeLineConstants.Straight;
        private INodeLineCalculator? _lineCalculator;
        public ILineCalculatorFactory _lineCalculatorFactory;

        public INotifyPosition? Source {  get; private set; }

        public INotifyPosition? Target { get; private set; }

        public string Mode
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

        public NodeLine( INotifyPosition source, INotifyPosition target, ILineCalculatorFactory lineCalculatorFactory)
        {
            Source = source;
            Target = target;
            _lineCalculatorFactory = lineCalculatorFactory;
            _lineCalculator = _lineCalculatorFactory?.GetCalculator(_mode);

            Data = new PathGeometry();
            Canvas.SetZIndex(this, -100);
            WeakEventManager<INotifyPosition, PositionChangedEventArgs>.AddHandler(Source, nameof(INotifyPosition.PositionChangedEventHandler), OnNodeMove);
            WeakEventManager<INotifyPosition, PositionChangedEventArgs>.AddHandler(Target, nameof(INotifyPosition.PositionChangedEventHandler), OnNodeMove);
            Recalculate();
            _lineCalculatorFactory = lineCalculatorFactory;
        }

        private void OnNodeMove(object? notifier, PositionChangedEventArgs args)
        {
            Recalculate();
        }

        private void OnModeChanged()
        {
            _lineCalculator = _lineCalculatorFactory?.GetCalculator(_mode);
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
