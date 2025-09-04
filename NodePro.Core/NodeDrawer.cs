using NodePro.Core.Model;
using NodePro.Core.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NodePro.Core
{
    public class NodeDrawer
    {
        private readonly IContainerProvider _containerProvider;
        private readonly NodeCreator _creator;

        private readonly Canvas _canvas;

        private NodeContainer? _selectedContainer = null;
        public NodeDrawer(IContainerProvider containerProvider, Canvas canvas)
        {
            _containerProvider = containerProvider;
            _creator = containerProvider.Resolve<NodeCreator>();
            _canvas = canvas;
        }

        #region Public Method

        public void DrawNode<TSheet>(double x, double y) where TSheet : NodeSheet => DrawNode<TSheet>(new System.Windows.Point(x, y));

        public void DrawNode<TSheet>(Point position) where TSheet : NodeSheet
        {
            NodeInitArgs args = new()
            {
                Position = position,
                NodeConnect = OnNodeConnect
            };
            NodeContainer container = _creator.CreateContainer<TSheet>(args);
            AddToCanvas(container);
        }

        public  void DrawConnect(NodeConnectEventArgs args)
        {

        }

        private void OnNodeConnect(object sender, NodeConnectEventArgs args)
        {
            DrawConnect(args);
        }


        public void AddToCanvas(UIElement element)
        {
            _canvas.Children.Add(element);
        }





        #endregion


    }
}
