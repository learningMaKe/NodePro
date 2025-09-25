using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NodePro.Abstractions
{
    public abstract class NodeConnectorBase : Control, INodeConnector
    {
        public abstract NodeContainerBase NodeParent { get; set; }
        public abstract ConnectorType ConnectorType { get; set; }
        public abstract Point Position { get; set; }

        public abstract event PositionChangedEventHandler? PositionChangedEventHandler;

        public abstract void OnConnectStart();
    }
}
