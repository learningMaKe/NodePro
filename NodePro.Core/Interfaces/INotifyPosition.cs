using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodePro.Core.Interfaces
{
    public class PositionChangedEventArgs : EventArgs
    {
        public Point OldPosition;
        public Point NewPosition;
    }

    public delegate void PositionChangedEventHandler(INotifyPosition notifier, PositionChangedEventArgs args);
    public interface INotifyPosition
    {
        public PositionChangedEventHandler? PositionChangedEventHandler { get; set; }

        public Point Position { get; }
    }
}
