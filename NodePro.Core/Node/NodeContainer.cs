using NodePro.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace NodePro.Core.Node
{
    [ContentProperty("Elements")]
    public class NodeContainer:Expander
    {
        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(NodeContainer), new PropertyMetadata(null));



        public NodeElementGroup Elements
        {
            get { return (NodeElementGroup)GetValue(ElementsProperty); }
            set { SetValue(ElementsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Elements.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ElementsProperty =
            DependencyProperty.Register("Elements", typeof(NodeElementGroup), typeof(NodeContainer), new PropertyMetadata(null));



        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(NodeContainer), new PropertyMetadata(false));

        #region Delegate Command
        public static DelegateCommand<NodeContainer> HeaderClickedCommand { get; } = new(ExecuteHeaderClickedCommand);


        #endregion

        #region Routed Event

        #region Selected Event
        public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent(
            "Selected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NodeContainer));

        

        public event RoutedEventHandler Selected
        {
            add { AddHandler(SelectedEvent, value); }
            remove { RemoveHandler(SelectedEvent, value); }
        }

        public void OnSelected()
        {
            IsSelected = true;
            RoutedEventArgs args = new RoutedEventArgs(SelectedEvent);
            RaiseEvent(args);
        }
        #endregion

        #region UnSelected Event

        public static readonly RoutedEvent UnSelectedEvent = EventManager.RegisterRoutedEvent(
            "UnSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NodeContainer));

        public event RoutedEventHandler UnSelected
        {
            add { AddHandler(UnSelectedEvent, value); }
            remove { RemoveHandler(UnSelectedEvent, value); }
        }

        public void OnUnSelected()
        {
            IsSelected = false;
            RoutedEventArgs args = new RoutedEventArgs(UnSelectedEvent);
            RaiseEvent(args);
        }

        #endregion
            #endregion
        public NodeContainer()
        {
            Elements = [];
        }

        private static void ExecuteHeaderClickedCommand(NodeContainer container)
        {
            if (container == null) return;
            if (container.IsSelected)
            {
                container.OnUnSelected();
            }
            else
            {
                container.OnSelected();
            }

        }




    }
}
