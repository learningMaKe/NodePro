using NodePro.Abstractions;
using NodePro.Abstractions.Arguments;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Interfaces;
using NodePro.Abstractions.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace NodePro.Core.Node
{
    public class MoveEventArgs(RoutedEvent routedEvent,NodeContainer container,DragDeltaEventArgs args) : RoutedEventArgs(routedEvent)
    {
        public NodeContainer Container { get; } = container ?? throw new ArgumentNullException(nameof(container));

        public DragDeltaEventArgs Args { get; } = args ?? throw new ArgumentNullException(nameof(args));
    }

    public delegate void NodeConnectEventHandler(object sender, NodeConnectEventArgs args);

    public delegate void MoveEventHandler(object container, MoveEventArgs args);

    public delegate void NodeConnectStartEventHandler(object sender, NodeConnectStartEventArgs args);

    [ContentProperty("Elements")]
    public class NodeContainer:Expander,INodeContainer
    {

        #region Fields

        #endregion

        #region Properties

        public INodeSheet? Sheet { get; set; }

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
                Notifier = container,
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
        public DelegateCommand<DragDeltaEventArgs> MoveCommand { get; }


        #endregion

        #region Routed Event

        #region Selected Event

        public static readonly RoutedEvent SelectedEvent =  EventManager.RegisterRoutedEvent(
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

        #region TransmitData Event

        public static readonly RoutedEvent TransmitDataEvent = EventManager.RegisterRoutedEvent("TransmitData",RoutingStrategy.Bubble,typeof(RoutedEventHandler),typeof(NodeContainer));

        public event RoutedEventHandler TransmitData
        {
            add { AddHandler(TransmitDataEvent, value); }
            remove { AddHandler(TransmitDataEvent, value); }
        }

        public void OnTransmitData()
        {
            RoutedEventArgs args = new RoutedEventArgs(TransmitDataEvent);
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

        public static readonly RoutedEvent NodeConnectStartEvent = EventManager.RegisterRoutedEvent("NodeConnectStart", RoutingStrategy.Bubble, typeof(NodeConnectStartEventHandler), typeof(NodeContainer));

        public event NodeConnectStartEventHandler NodeConnectStart
        {
            add { AddHandler(NodeConnectStartEvent, value); }
            remove { RemoveHandler (NodeConnectStartEvent, value); }
        }
        private void OnNodeConnectStart(ConnectStartEventArgs args)
        {
            ConnectionEndpoint endpoint = new ConnectionEndpoint(this, args.From);
            NodeConnectStartEventArgs e = new(NodeConnectStartEvent, endpoint);
            RaiseEvent(e);
        }
        #endregion

        #region Connector Event

        public static readonly RoutedEvent ConnectEvent = NodeConnector.ConnectEvent.AddOwner(typeof(NodeContainer));

        public event ConnectEventHandler Connect
        {
            add { AddHandler(ConnectEvent, value); }
            remove { RemoveHandler(ConnectEvent, value); }
        }

        public static readonly RoutedEvent ConnectStartEvent = NodeConnector.ConnectStartEvent.AddOwner(typeof(NodeContainer));

        public event ConnectStartEventHandler ConnectStart
        {
            add { AddHandler(ConnectStartEvent, value); }
            remove { RemoveHandler(ConnectStartEvent, value); }
        }

        #endregion
        #endregion

        #region Events

        public event PositionChangedEventHandler? PositionChangedEventHandler;

        #endregion

        #region Constructor
        public NodeContainer()
        {
            Elements = [];
            ElementSelector ??= new NodeTemplateSelector();
            ElementSelector.TemplateSelected += OnTemplateSelected;

            MoveCommand = new(ExecuteMoveCommand);
            HeaderClickedCommand = new(ExecuteHeaderClickedCommand);
            Connect += ExecuteConnect;
            ConnectStart += ExecuteConnectStart;
        }

        #endregion

        public void Execute(NodeExecutionData data)
        {
            Sheet?.DataProcess(data.SharedData);
        }
        private void ExecuteConnect(object sender, ConnectEventArgs e)
        {
            OnNodeConnect(e);
        }

        private void ExecuteConnectStart(object sender, ConnectStartEventArgs e)
        {
            OnNodeConnectStart(e);
        }


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
            else if(this.TryFindResource(NodeTemplateKey.DefaultNodeTemplate) is DataTemplate defaultTemplate)
            {
                return defaultTemplate;
            }
            return null;
        }

        private void ExecuteHeaderClickedCommand()
        {
            OnClicked();
        }

        private void ExecuteMoveCommand(DragDeltaEventArgs args)
        {
            this.OnMove(args);

        }

    }
}
