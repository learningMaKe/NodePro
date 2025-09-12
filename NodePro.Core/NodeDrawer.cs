using NodePro.Core.Interfaces;
using NodePro.Core.Model;
using NodePro.Core.Node;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NodePro.Core
{
    public class NodeDrawer
    {
        private readonly IContainerProvider _containerProvider;
        private readonly NodeCreator _creator;
        private readonly NodeCanvas _canvas;

        public NodeDrawer(IContainerProvider containerProvider, NodeCanvas canvas)
        {
            _containerProvider = containerProvider;
            _creator = containerProvider.Resolve<NodeCreator>();
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
            NodeLine line = new(args);
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

        }

        private void OnNodeConnectStart(object sender, NodeConnectStartEventArgs args)
        {
            _connectStartArgs = args;
            StartTrack(args.StartFrom.Connector);
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
            _trackLine = new(notify, _tracker);
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

        #endregion
    }
}
