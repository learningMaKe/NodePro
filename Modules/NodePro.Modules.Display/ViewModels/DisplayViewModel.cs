using NodePro.Core;
using NodePro.Core.MVVM;
using NodePro.Core.Node;
using NodePro.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NodePro.Modules.Display.ViewModels
{
    public class DisplayViewModel : ViewModelBase
    {
        private NodeDrawer? _drawer = null;
        private INodeService _nodeService;

        public DelegateCommand<NodeConnectEventArgs> ConnectCommand { get; }

        #region Public Method

        private readonly DelegateCommand<Canvas>? _canvasLoadedCommand = null;
        public DelegateCommand<Canvas> CanvasLoadedCommand =>
            _canvasLoadedCommand ?? new DelegateCommand<Canvas>(ExecuteCanvasLoadedCommand);

        void ExecuteCanvasLoadedCommand(Canvas parameter)
        {
            _drawer ??= new NodeDrawer(_containerProvider, parameter);

            _drawer.DrawNode<DemoSheet>(500, 500);
            _drawer.DrawNode<DemoSheet>(700, 500);
            _drawer.DrawNode<DemoSheet>(900, 500);
        }

        #endregion

        public DisplayViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {
            _nodeService = containerProvider.Resolve<INodeService>();
            ConnectCommand = new(ExecuteConnectCommand);
        }

        private void ExecuteConnectCommand(NodeConnectEventArgs args)
        {
            Debug.WriteLine($"Connect from {args.NodeSource} to {args.NodeTarget}");
        }
    }
}
