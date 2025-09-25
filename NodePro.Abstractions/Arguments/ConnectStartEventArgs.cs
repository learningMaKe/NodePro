using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodePro.Abstractions.Arguments
{
    public class ConnectStartEventArgs : RoutedEventArgs
    {
        public NodeConnectorBase From { get; private set; }
        public ConnectStartEventArgs(RoutedEvent routed, NodeConnectorBase from) : base(routed)
        {
            From = from;
        }

    }
}
