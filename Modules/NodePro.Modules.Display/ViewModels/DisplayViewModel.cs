using NodePro.Core.MVVM;
using NodePro.Core.Node;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Modules.Display.ViewModels
{
    public class DisplayViewModel : ViewModelBase
    {
        public DelegateCommand<ConnectEventArgs> ConnectCommand { get; }
        public DisplayViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {
            ConnectCommand = new(ExecuteConnectCommand);
        }

        private void ExecuteConnectCommand(ConnectEventArgs args)
        {
            Debug.WriteLine($"Connect from {args.SourceConnector} to {args.TargetConnector}");
        }
    }
}
