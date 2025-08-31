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
using System.Windows.Shapes;

namespace NodePro.Core.Node
{
    public enum ConnectorType
    {
        Input,
        Output
    }

    public class ConnectEventArgs : RoutedEventArgs
    {
        public NodeConnector SourceConnector { get; }
        public NodeConnector TargetConnector { get; }
        public ConnectEventArgs(RoutedEvent routedEvent, NodeConnector source, NodeConnector target) : base(routedEvent)
        {
            SourceConnector = source;
            TargetConnector = target;
        }
    }

    public delegate void ConnectEventHandler(object sender, ConnectEventArgs e);

    public class NodeConnector : Control
    {
        #region Properties

        private Point _position = new Point();
        private bool _isDrag = false;

        #endregion

        #region Dependency Properties
        public ConnectorType ConnectorType
        {
            get { return (ConnectorType)GetValue(ConnectorTypeProperty); }
            set { SetValue(ConnectorTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConnectorTypeProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectorTypeProperty =
            DependencyProperty.Register("ConnectorType", typeof(ConnectorType), typeof(NodeConnector), new PropertyMetadata(ConnectorType.Input));

        public double ConnectorSize
        {
            get { return (double)GetValue(ConnectorSizeProperty); }
            set { SetValue(ConnectorSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConnectorSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectorSizeProperty =
            DependencyProperty.Register("ConnectorSize", typeof(double), typeof(NodeConnector), new PropertyMetadata(15.0));



        public NodeContainer NodeParent
        {
            get { return (NodeContainer)GetValue(NodeParentProperty); }
            set { SetValue(NodeParentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NodeParent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NodeParentProperty =
            DependencyProperty.Register("NodeParent", typeof(NodeContainer), typeof(NodeConnector), new PropertyMetadata(null));



        #endregion

        #region Delegate Command

        public static DelegateCommand<DragEventArgs> DropCommand { get; } = new DelegateCommand<DragEventArgs>(ExecuteDropCommand);
        public static DelegateCommand<MouseButtonEventArgs> DropStartCommand { get; } = new DelegateCommand<MouseButtonEventArgs>(ExecuteDropStartCommand);

        public static DelegateCommand<MouseEventArgs> DropMoveCommand { get; } = new DelegateCommand<MouseEventArgs>(ExecuteDropMoveCommand);
        #endregion

        #region Routed Event

        #region ConnectEvent

        // 注册路由事件：使用自定义的 ConnectEventHandler 委托类型
        public static readonly RoutedEvent ConnectEvent = EventManager.RegisterRoutedEvent(
            "Connect",                  // 事件名称
            RoutingStrategy.Bubble,     // 路由策略（冒泡）
            typeof(ConnectEventHandler),// 事件处理委托类型（匹配自定义参数）
            typeof(NodeConnector)       // 事件所属类型
        );

        // 暴露 CLR 事件包装器（可选，但推荐，便于在 XAML 中绑定）
        public event ConnectEventHandler Connect
        {
            add => AddHandler(ConnectEvent, value);
            remove => RemoveHandler(ConnectEvent, value);
        }

        // 触发事件的方法（内部调用，用于发布事件）
        protected virtual void OnConnect(NodeConnector source, NodeConnector target)
        {
            // 创建自定义事件参数，传递路由事件和数据
            var args = new ConnectEventArgs(ConnectEvent, source, target);
            // 触发事件（this 为事件源）
            RaiseEvent(args);
        }

        #endregion

        #region Drop Event


        #endregion

        #endregion


        static NodeConnector()
        {

        }

        private static void ExecuteDropCommand(DragEventArgs args)
        {
            // 验证源对象
            if (args.Source is not DependencyObject obj) return;

            // 获取拖拽的源连接器
            if (args.Data.GetData(typeof(NodeConnector)) is not NodeConnector source) return;

            // 获取目标连接器
            var target = NodeHelper.GetConnector(obj);
            if (target == null) return;

            // 1. 检查源和目标是否属于同一个父节点（防止节点自连）
            if (source.NodeParent == target.NodeParent)
            {
                System.Diagnostics.Debug.WriteLine("无效连接：不能在同一节点的连接器之间建立连接");
                return;
            }

            // 2. 声明实际用于连接的源和目标
            NodeConnector actualSource;
            NodeConnector actualTarget;

            // 3. 判断连接方向并处理
            if (source.ConnectorType == ConnectorType.Output && target.ConnectorType == ConnectorType.Input)
            {
                // 情况1：正常方向（输出→输入），直接使用原始对象
                actualSource = source;
                actualTarget = target;
            }
            else if (source.ConnectorType == ConnectorType.Input && target.ConnectorType == ConnectorType.Output)
            {
                // 情况2：反向连接（输入→输出），自动反转方向
                actualSource = target;  // 输出节点作为实际源
                actualTarget = source;  // 输入节点作为实际目标
            }
            else
            {
                // 情况3：无效连接（输入→输入 或 输出→输出）
                System.Diagnostics.Debug.WriteLine("无效的连接组合：只能在输出和输入之间建立连接");
                return;
            }

            // 4. 执行连接操作（确保始终是输出→输入，且不同节点）
            actualTarget.OnConnect(actualSource, actualTarget);
        }

        private static void ExecuteDropStartCommand(MouseButtonEventArgs args)
        {
            if (args.Source is not DependencyObject obj) return;
            var source = NodeHelper.GetConnector(obj);
            if (source == null) return;
            source._position = args.GetPosition(null);
            source._isDrag = true;
        }

        private static void ExecuteDropMoveCommand(MouseEventArgs args)
        {
            if (args.LeftButton != MouseButtonState.Pressed) return;
            if (args.Source is not DependencyObject obj) return;
            var source = NodeHelper.GetConnector(obj);
            if (source == null) return;
            Point start = source._position;
            Point current = args.GetPosition(null);
            double horizontalDiff = Math.Abs(current.X - start.X);
            double verticalDiff = Math.Abs(current.Y - start.Y);

            // 当水平或垂直距离超过系统默认阈值时，启动拖放
            if (horizontalDiff > SystemParameters.MinimumHorizontalDragDistance ||
                verticalDiff > SystemParameters.MinimumVerticalDragDistance)
            {
                source._position = current;
                DragDrop.DoDragDrop(obj, source, DragDropEffects.Move);


            }
        }
    }
}
