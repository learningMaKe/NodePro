using DryIoc.ImTools;
using NodePro.Abstractions;
using NodePro.Abstractions.Arguments;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Exceptions;
using NodePro.Abstractions.Interfaces;
using NodePro.Abstractions.Models;
using NodePro.Core.Node;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NodePro.Core
{
    public class NodeDrawer
    {

        #region Fields

        private readonly IContainerProvider _containerProvider;
        private readonly NodeCreateService _creator;
        private readonly NodeLineCreator _lineCreator;
        private readonly NodeCanvas _canvas;

        private readonly List<LinePair> _linePairs = [];
        private string _lineMode = NodeLineConstants.Curve;

        #endregion

        #region Properties


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

        #endregion

        #region Constructor

        public NodeDrawer(IContainerProvider containerProvider, NodeCanvas canvas)
        {
            _containerProvider = containerProvider;
            _creator = containerProvider.Resolve<NodeCreateService>();
            _lineCreator = containerProvider.Resolve<NodeLineCreator>();
            _canvas = canvas;
            InitCanvas();
        }
        #endregion

        #region Public Method

        public void DrawNode<TSheet>(double x, double y) where TSheet : INodeSheet => DrawNode<TSheet>(new System.Windows.Point(x, y));

        public void DrawNode<TSheet>(Point position) where TSheet : INodeSheet
        {
            NodeInitArgs args = new()
            {
                Position = position,
            };
            NodeContainer? container = _creator.CreateContainer<TSheet>(args);
            if (container is null) return;
            AddToCanvas(container);
        }


        public NodeLine DrawConnect(NodeConnectEventArgs args)
        {
            NodeLine line = _lineCreator.CreateLine(args);
            AddToCanvas(line);
            return line;
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

        private void InitCanvas()
        {
            _canvas.NodeConnect += OnNodeConnect;
            _canvas.NodeConnectStart += OnNodeConnectStart;
            _canvas.MouseMove += OnMouseMove;
            _canvas.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            _canvas.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            _canvas.Drop += OnCanvasDrop;
            _canvas.TransmitData += ExecuteTransmitData;
            InitTrack();
        }

        private void ExecuteTransmitData(object sender, RoutedEventArgs e)
        {
            if (sender is not NodeContainer nodeContainer) return;
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            INodeContainer[] containers = GetContainers();
            containers.ForEach(x => x.IsSelected = false);
        }

        private void OnCanvasDrop(object sender, DragEventArgs e)
        {
            Debug.WriteLine("Canvas Dropped");
            INodeConnector? connector= e.Data.GetData(typeof(INodeConnector)) as INodeConnector;
            if (connector is null) return;
        }

        /// <summary>
        /// 连线的核心方法，此处调用<see cref="DragDrop.DoDragDrop(DependencyObject, object, DragDropEffects)"/> 放下数据
        /// 数据类型 <see cref="NodeData"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_tracking)
            {
                EndTrack();

                if (_connectStartArgs == null) return;
                NodeData data = [];
                NodeDropArgs args = new NodeDropArgs()
                {
                    StartFrom = _connectStartArgs.StartFrom.Connector,
                };
                data.Add(NodeConstants.ParamsDrop, args);
                DragDrop.DoDragDrop(_canvas, data, DragDropEffects.Move);
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
            INodeConnector start = pair.Source.Connector;
            pair.Target.Connector.OnConnectRelease();
            _linePairs.Remove(pair);
            RemoveFromCanvas(pair.Line as Control);
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

        private List<T> GetCanvasTypes<T>()
        {
            List<T> elements = [];
            foreach (var element in _canvas.Children) 
            {
                if (element is not T t) continue;
                elements.Add(t);
            }
            return elements;
        }

        private INodeContainer[] GetContainers() => GetCanvasTypes<INodeContainer>().ToArray();

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
