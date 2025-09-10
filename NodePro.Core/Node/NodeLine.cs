using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NodePro.Core.Node
{
    public class NodeLine:Control
    {
        private readonly NodeConnectEventArgs? _args;
        private LineCalculateMode _mode = LineCalculateMode.Straight;
        private INodeLineCalculator? _lineCalculator;

        public ConnectionEndpoint Source {  get; private set; }

        public ConnectionEndpoint Target { get; private set; }

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




        public NodeLine(NodeConnectEventArgs args)
        {
            _args = args;
            _lineCalculator = LineCalculatorFactory.GetCalculator(_mode);
            Data = new PathGeometry();
            Source = args.NodeSource;
            Target = args.NodeTarget;

            // Source.Node.Move += OnNodeMove;
            // Target.Node.Move += OnNodeMove;
            WeakEventManager<NodeContainer, MoveEventArgs>.AddHandler(Source.Node, "Move", OnNodeMove);
            WeakEventManager<NodeContainer, MoveEventArgs>.AddHandler(Target.Node, "Move", OnNodeMove);
            Recalculate();
        }

        public NodeLine(Point source,Point target)
        {

        }

        private void OnNodeMove(object? container, MoveEventArgs args)
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
            PathFigure figure = _lineCalculator.Calculate(Source.Connector.Position, Target.Connector.Position);
            Data.Figures.Add(figure);

        }
    }
}
