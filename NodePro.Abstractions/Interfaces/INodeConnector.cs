using NodePro.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Interfaces
{
    public interface INodeConnector:INotifyPosition
    {
        public NodeContainerBase NodeParent { get; set; }

        public ConnectorType ConnectorType { get; set; }

        public void OnConnectStart();
    }
}
