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
    public abstract class NodeContainerBase : Expander, INodeContainer
    {
        public abstract Point Position { get; set; }

        public abstract event PositionChangedEventHandler? PositionChangedEventHandler;
    }
}
