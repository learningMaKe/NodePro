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
        private readonly Canvas _canvas;

        private NodeContainer? _selectedContainer = null;
        public NodeDrawer(Canvas canvas)
        {
            _canvas = canvas;
        }

        #region Public Method

        public void DrawNode<TSheet>(Point position, TSheet sheet) where TSheet : NodeSheet
        {
            NodeModel model = sheet.CreateModel();
            NodeContainer container = new NodeContainer()
            {
                Position = position,
                Header = model.Title,
                Elements = model.Elements,
            };
            container.NodeConnect += OnNodeConnect;
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
