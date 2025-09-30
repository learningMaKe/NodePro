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
        public INodeConnector SourceConnector { get; }
        public INodeConnector TargetConnector { get; }
        public ConnectEventArgs(RoutedEvent routedEvent, INodeConnector source, INodeConnector target) : base(routedEvent)
        {
            SourceConnector = source;
            TargetConnector = target;
        }
    }
}
