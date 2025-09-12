using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NodePro.Core.Node
{
    public class NodeCanvas:Canvas
    {
        public static readonly RoutedEvent NodeConnectEvent = NodeContainer.NodeConnectEvent.AddOwner(typeof(NodeCanvas));

        public event NodeConnectEventHandler NodeConnect
        {
            add { AddHandler(NodeConnectEvent, value); }
            remove { RemoveHandler(NodeConnectEvent, value); }
        }

        public static readonly RoutedEvent NodeConnectStartEvent = NodeContainer.NodeConnectStartEvent.AddOwner(typeof(NodeCanvas));

        public event NodeConnectStartEventHandler NodeConnectStart
        {
            add { AddHandler(NodeConnectStartEvent, value); }
            remove { RemoveHandler(NodeConnectStartEvent, value); }
        }

        public NodeCanvas()
        {
            
        }
    }
}
