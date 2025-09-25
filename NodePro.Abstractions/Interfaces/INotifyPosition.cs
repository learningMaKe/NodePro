using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodePro.Abstractions.Interfaces
{
    public class PositionChangedEventArgs : EventArgs
    {
        public required INotifyPosition Notifier;
        public Point OldPosition;
        public Point NewPosition;

    }

    public delegate void PositionChangedEventHandler(object notifier, PositionChangedEventArgs args);

    public interface INotifyPosition
    {
        public event PositionChangedEventHandler? PositionChangedEventHandler;

        public Point Position { get; set; }
    }
}
