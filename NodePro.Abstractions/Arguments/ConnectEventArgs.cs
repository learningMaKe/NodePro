using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodePro.Abstractions.Arguments
{
    public class ConnectEventArgs : RoutedEventArgs
    {
        public NodeConnectorBase SourceConnector { get; }
        public NodeConnectorBase TargetConnector { get; }
        public ConnectEventArgs(RoutedEvent routedEvent, NodeConnectorBase source, NodeConnectorBase target) : base(routedEvent)
        {
            SourceConnector = source;
            TargetConnector = target;
        }
    }
}
