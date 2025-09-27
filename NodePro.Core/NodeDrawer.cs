using NodePro.Abstractions;
using NodePro.Abstractions.Arguments;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Exceptions;
using NodePro.Abstractions.Interfaces;
using NodePro.Abstractions.Models;
using NodePro.Core.Node;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NodePro.Core
{
    

    public class NodeDrawer
    {
        private readonly IContainerProvider _containerProvider;
        private readonly NodeCreator _creator;
        private readonly NodeLineCreator _lineCreator;
        private readonly NodeCanvas _canvas;

        private readonly List<LinePair> _linePairs = [];

        private string _lineMode = NodeLineConstants.Curve;
        public string LineMode
        {
            get => _lineMode;
            set 
            { 
                _lineMode = value;
                _lineCreator.LineMode = value;
                OnLineModeChanged();
            }
        }
        public NodeDrawer(IContainerProvider containerProvider, NodeCanvas canvas)
        {
            _containerProvider = containerProvider;
            _creator = containerProvider.Resolve<NodeCreator>();
            _lineCreator = containerProvider.Resolve<NodeLineCreator>();
            _canvas = canvas;
            InitCanvas();
        }

        #region Public Method

        public void DrawNode<TSheet>(double x, double y) where TSheet : NodeSheet => DrawNode<TSheet>(new System.Windows.Point(x, y));

        public void DrawNode<TSheet>(Point position) where TSheet : NodeSheet
        {
            NodeInitArgs args = new()
            {
                Position = position,
            };
            NodeContainer container = _creator.CreateContainer<TSheet>(args);
            AddToCanvas(container);
        }

        public NodeLine DrawConnect(NodeConnectEventArgs args)
        {
            NodeLine line = _lineCreator.CreateLine(args);
            AddToCanvas(line);
            return line;
        }

        public void ExecuteFrom(NodeContainer node)
        {
            NodeData data = new NodeData();
            
        }

        public void AddToCanvas(UIElement element)
        {
            _canvas.Children.Add(element);
        }

        public void RemoveFromCanvas(UIElement? element)
        {
            if (element is null) return;
            _canvas.Children.Remove(element);
        }

        #endregion

        #region Private Methods

        private void CreateExecutionGroup()
        {

        }
        private void InitCanvas()
        {
            _canvas.NodeConnect += OnNodeConnect;
            _canvas.NodeConnectStart += OnNodeConnectStart;
            _canvas.MouseMove += OnMouseMove;
            _canvas.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;

            InitTrack();
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_tracking)
            {
                EndTrack();

                if (_connectStartArgs == null) return;
                DragDrop.DoDragDrop(_canvas, _connectStartArgs.StartFrom.Connector, DragDropEffects.Move);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_tracking) return;
            if (_tracker == null) return;
            _tracker.Position = e.GetPosition(_canvas);

        }

        private void OnNodeConnect(object sender, NodeConnectEventArgs args)
        {
            NodeLine line = DrawConnect(args);
            LinePair pair = new LinePair(args, line);
            _linePairs.Add(pair);
        }

        private void OnNodeConnectStart(object sender, NodeConnectStartEventArgs args)
        {
            void DoConnectFrom(INodeConnector connector)
            {
                _connectStartArgs = args;
                StartTrack(connector);
            }
            ConnectorType type = args.StartFrom.Connector.ConnectorType;
            // 输出点支持多次向外连线
            if (type == ConnectorType.Output)
            {
                DoConnectFrom(args.StartFrom.Connector);
                return;
            }
            // 输入点仅支持一次连线
            LinePair[] pairs = _linePairs.Where(x => x.Source.Connector == args.StartFrom.Connector || x.Target.Connector == args.StartFrom.Connector).ToArray();
            // 如果当前没有连线直接允许
            if (pairs.Length <= 0)
            {
                DoConnectFrom(args.StartFrom.Connector);
                return;
            }
            // 取第一个，多余一个都是意外情况
            if (pairs.Length > 1) 
            {
                throw new NodeConnectException(ConnectionErrorCode.输入点多条连线);
            }
            LinePair pair = pairs.FirstOrDefault();
            NodeConnectorBase start = pair.Source.Connector;
            _linePairs.Remove(pair);
            RemoveFromCanvas(pair.Line);
            // 不能使用DoConnectFrom,因为这里调整了流向，args.StartFrom来自输入点，但start来自输出点，流向有问题，要重开一个NodeConnect事件
            start.OnConnectStart();


        }

        #region Track Method
        private bool _tracking = false;
        private NodeTracker? _tracker;
        private NodeLine? _trackLine;
        private NodeConnectStartEventArgs? _connectStartArgs;

        private void InitTrack()
        {
            if (_tracker is null)
            {
                _tracker = new NodeTracker()
                {
                    Position = Mouse.GetPosition(_canvas)
                };
                AddToCanvas(_tracker);
            }
            _tracking = false;
        }

        private void StartTrack(INotifyPosition notify)
        {
            if (_tracker == null) return;
            _tracker.Position = Mouse.GetPosition(_canvas);
            _trackLine = _lineCreator.CreateLine(notify, _tracker);
            AddToCanvas(_trackLine);
            _tracking = true;
        }

        private void EndTrack()
        {
            if(_tracker == null) return;
            _tracking = false;
            RemoveFromCanvas(_trackLine);
            _trackLine = null;
        }

        #endregion

        private List<T> GetCanvasTypes<T>() where T :UIElement
        {
            List<T> elements = [];
            foreach (var element in _canvas.Children) 
            {
                if (element.GetType() != typeof(T)) continue;
                elements.Add((T)element);
            }
            return elements;
        }

        private void OnLineModeChanged()
        {
            List<NodeLine> lines = GetCanvasTypes<NodeLine>();
            foreach (var line in lines)
            {
                line.Mode = LineMode;
            }
        }
        #endregion
    }
}
