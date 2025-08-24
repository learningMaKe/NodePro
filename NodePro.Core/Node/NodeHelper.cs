using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodePro.Core.Node
{
    public class NodeHelper
    {


        public static NodeConnector GetConnector(DependencyObject obj)
        {
            return (NodeConnector)obj.GetValue(ConnectorProperty);
        }

        public static void SetConnector(DependencyObject obj, NodeConnector value)
        {
            obj.SetValue(ConnectorProperty, value);
        }

        // Using a DependencyProperty as the backing store for Connector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectorProperty =
            DependencyProperty.RegisterAttached("Connector", typeof(NodeConnector), typeof(NodeHelper), new PropertyMetadata(null));


    }
}
