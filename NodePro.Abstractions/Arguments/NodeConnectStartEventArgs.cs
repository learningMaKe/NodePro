using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodePro.Abstractions.Arguments
{
    public class NodeConnectStartEventArgs : RoutedEventArgs
    {
        public ConnectionEndpoint StartFrom { get; init; }
        public NodeConnectStartEventArgs(RoutedEvent routed, ConnectionEndpoint endpoint) : base(routed)
        {
            StartFrom = endpoint;
        }
    }
}
