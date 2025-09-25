using NodePro.Abstractions.Events;
using System.ComponentModel;
using Wpf.Ui;

namespace NodePro.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private IEventAggregator _eventAggregator;

        public MainWindow(IEventAggregator eventAggregator, IContentDialogService dialogService)
        {
            _eventAggregator = eventAggregator;
            InitializeComponent();

            _eventAggregator.GetEvent<ExitEvent>().Subscribe(OnExitEvent);
            dialogService.SetDialogHost(RootContentDialog);
            this.Closing += OnWindowClosing;
        }

        private void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            CloseEventRegister reg = new();
            _eventAggregator.GetEvent<ClosingEvent>().Publish(reg);
            if (reg.Count == 0) return;
            e.Cancel = true;
            Task.Factory.StartNew(async () =>
            {
                bool state = await reg.Execute();
                if (state)
                {
                    this.Close();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnExitEvent()
        {
            this.Close();
        }
    }
}
