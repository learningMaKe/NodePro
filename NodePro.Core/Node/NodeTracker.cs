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
    public class NodeTracker:Control
    {
        private readonly NodeConnectEventArgs _args;
        private LineCalculateMode _mode = LineCalculateMode.Straight;
        private INodeLineCalculator _lineCalculator;

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
            DependencyProperty.Register("Data", typeof(PathGeometry), typeof(NodeTracker), new PropertyMetadata(null));




        public NodeTracker(NodeConnectEventArgs args)
        {
            _args = args;
            _lineCalculator = LineCalculatorFactory.GetCalculator(_mode);
            Source = args.NodeSource;
            Target = args.NodeTarget;

            Source.Node.Move += OnNodeMove;
            Target.Node.Move += OnNodeMove;

        }

        private void OnNodeMove(object container, MoveEventArgs args)
        {
            
        }

        private void OnModeChanged()
        {
            _lineCalculator = LineCalculatorFactory.GetCalculator(_mode);
        }
    }
}
