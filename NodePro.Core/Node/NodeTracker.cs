using NodePro.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NodePro.Core.Node
{
    public class NodeTracker : Control, INotifyPosition
    {
        private Point _position = new Point();
        private PositionChangedEventArgs _positionChangedEventArgs;

        public Point Position
        {
            get => _position;
            set
            {
                _positionChangedEventArgs.OldPosition = _position;
                _positionChangedEventArgs.NewPosition = value;
                _position = value;
                PositionChangedEventHandler?.Invoke(this, _positionChangedEventArgs);
            }
        }

        public NodeTracker()
        {
            _positionChangedEventArgs = new() { Notifier = this };
        }

        public event PositionChangedEventHandler? PositionChangedEventHandler;
    }
}
