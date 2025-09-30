using NodePro.Abstractions.Arguments;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Models;
using System.Windows;

namespace NodePro.Abstractions.Interfaces
{

    public delegate void ConnectEventHandler(object sender, ConnectEventArgs e);

    public delegate void ConnectStartEventHandler(object sender, ConnectStartEventArgs e);

    public interface INodeConnector:INotifyPosition
    {
        public INodeContainer NodeParent { get; set; }

        public ConnectorType ConnectorType { get; set; }

        public NodeElement Element { get; set; }
        public void OnConnectStart();

        public void OnConnectRelease();

        public void OnConnect(INodeConnector source, INodeConnector target);

        public Point GetRealtivePosition();

    }
}
