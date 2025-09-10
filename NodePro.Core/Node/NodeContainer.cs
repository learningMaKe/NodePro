using NodePro.Core.Interfaces;
using NodePro.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    // 代表连接的一方（节点+连接器）
    public readonly struct ConnectionEndpoint(NodeContainer node, NodeConnector connector)
    {
        public NodeContainer Node { get; } = node ?? throw new ArgumentNullException(nameof(node));
        public NodeConnector Connector { get; } = connector ?? throw new ArgumentNullException(nameof(connector));
    }

    public class NodeConnectEventArgs(RoutedEvent routedEvent, ConnectEventArgs args) : RoutedEventArgs(routedEvent)
    {
        public ConnectionEndpoint NodeSource { get; } = new ConnectionEndpoint(args.SourceConnector.NodeParent, args.SourceConnector);

        public ConnectionEndpoint NodeTarget { get; } = new ConnectionEndpoint(args.TargetConnector.NodeParent, args.TargetConnector);
    }

    public class MoveEventArgs(RoutedEvent routedEvent,NodeContainer container,DragDeltaEventArgs args) : RoutedEventArgs(routedEvent)
    {
        public NodeContainer Container { get; } = container ?? throw new ArgumentNullException(nameof(container));

        public DragDeltaEventArgs Args { get; } = args ?? throw new ArgumentNullException(nameof(args));
    }

    public delegate void NodeConnectEventHandler(object sender, NodeConnectEventArgs args);

    public delegate void MoveEventHandler(object container, MoveEventArgs args);

    [ContentProperty("Elements")]
    public class NodeContainer:Expander,INotifyPosition
    {

        #region Fields

        #endregion

        #region Constants


        #endregion

        #region Dependency Property

        public int ZIndex
        {
            get { return (int)GetValue(ZIndexProperty); }
            set { SetValue(ZIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZIndexProperty =
            DependencyProperty.Register("ZIndex", typeof(int), typeof(NodeContainer), new PropertyMetadata(0,OnZIndexChanged));

        private static void OnZIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not NodeContainer container) return;
            Canvas.SetZIndex(container,(int)e.NewValue);
        }

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



        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Point), typeof(NodeContainer), new PropertyMetadata(new Point(),OnPositionChanged));

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not NodeContainer container) return;
            PositionChangedEventArgs args = new()
            {
                OldPosition = (Point)e.OldValue,
                NewPosition = (Point)e.NewValue,
            };
            container.PositionChangedEventHandler?.Invoke(container, args);
        }

        public NodeTemplateSelector ElementSelector
        {
            get { return (NodeTemplateSelector)GetValue(ElementSelectorProperty); }
            set { SetValue(ElementSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ElementSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ElementSelectorProperty =
            DependencyProperty.Register("ElementSelector", typeof(NodeTemplateSelector), typeof(NodeContainer), new PropertyMetadata(null));


        #endregion

        #region Delegate Command
        public DelegateCommand HeaderClickedCommand { get; }
        public static DelegateCommand<ConnectEventArgs> InputConnectCommand { get; } = new(ExecuteInputConnectCommand);
        public static DelegateCommand<ConnectEventArgs> OutputConnectCommand { get; } = new(ExecuteOutputConnectCommand);
        public DelegateCommand<DragDeltaEventArgs> MoveCommand { get; }


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

        #region Clicked Event

        public static readonly RoutedEvent ClickedEvent = EventManager.RegisterRoutedEvent("Clicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NodeContainer));

        public event RoutedEventHandler Clicked
        {
            add { AddHandler(ClickedEvent, value); }
            remove { RemoveHandler(ClickedEvent, value); }
        }

        private void OnClicked()
        {
            RoutedEventArgs args = new RoutedEventArgs(ClickedEvent);
            RaiseEvent(args);
        }

        #endregion

        #region Node Connect Event

        public static readonly RoutedEvent NodeConnectEvent = EventManager.RegisterRoutedEvent("NodeConnect", RoutingStrategy.Bubble, typeof(NodeConnectEventHandler), typeof(NodeContainer));
        
        public event NodeConnectEventHandler NodeConnect
        {
            add { AddHandler(NodeConnectEvent, value);}
            remove {  RemoveHandler(NodeConnectEvent, value); }
        }

        private void OnNodeConnect(ConnectEventArgs args)
        {
            NodeConnectEventArgs nodeConnectEventArgs = new NodeConnectEventArgs(NodeConnectEvent, args);
            RaiseEvent(nodeConnectEventArgs);
        }

        public static readonly RoutedEvent MoveEvent = EventManager.RegisterRoutedEvent("Move", RoutingStrategy.Bubble, typeof(MoveEventHandler), typeof(NodeContainer));
        
        public event MoveEventHandler Move
        {
            add { AddHandler(MoveEvent, value); }
            remove { RemoveHandler(MoveEvent, value); }
        }

        private void OnMove(DragDeltaEventArgs args)
        {
            if (!IsSelected)
            {
                OnSelected();
            }
            Position = new Point(Position.X + args.HorizontalChange, Position.Y + args.VerticalChange);
            MoveEventArgs moveArgs = new MoveEventArgs(MoveEvent, this, args);
            RaiseEvent(moveArgs);
        }

        #endregion

        #endregion

        #region Events

        public PositionChangedEventHandler? PositionChangedEventHandler { get; set; }

        #endregion

        #region Constructor
        public NodeContainer()
        {
            Elements = [];
            ElementSelector ??= new NodeTemplateSelector();
            ElementSelector.TemplateSelected += OnTemplateSelected;

            MoveCommand = new(ExecuteMoveCommand);
            HeaderClickedCommand = new(ExecuteHeaderClickedCommand);
        }

        #endregion

        private DataTemplate? OnTemplateSelected(NodeElement element, DependencyObject container)
        {
            if (this.TryFindResource(element.Template) is DataTemplate template)
            {
                return template;
            }
            else if (Application.Current.TryFindResource(element.Template) is DataTemplate appTemplate)
            {
                return appTemplate;
            }
            else if(this.TryFindResource(TemplateKey.DefaultNodeTemplate) is DataTemplate defaultTemplate)
            {
                return defaultTemplate;
            }
            return null;
        }

        private void ExecuteHeaderClickedCommand()
        {
            OnClicked();
        }

        private static void ExecuteInputConnectCommand(ConnectEventArgs args)
        {
            NodeContainer container = args.TargetConnector.NodeParent;
            if (container == null) return;
            container.OnNodeConnect(args);
        }

        private static void ExecuteOutputConnectCommand(ConnectEventArgs args)
        {
            Debug.WriteLine("Output Accepted");
        }
        private void ExecuteMoveCommand(DragDeltaEventArgs args)
        {
            this.OnMove(args);

        }


    }
}
