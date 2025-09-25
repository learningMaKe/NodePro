using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodePro.Abstractions.Arguments
{
    public class NodeConnectEventArgs(RoutedEvent routedEvent, ConnectEventArgs args) : RoutedEventArgs(routedEvent)
    {
        public ConnectionEndpoint NodeSource { get; } = new ConnectionEndpoint(args.SourceConnector.NodeParent, args.SourceConnector);

        public ConnectionEndpoint NodeTarget { get; } = new ConnectionEndpoint(args.TargetConnector.NodeParent, args.TargetConnector);
    }
}
