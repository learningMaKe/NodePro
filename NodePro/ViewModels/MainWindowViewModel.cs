using NodePro.Abstractions.Events;
using NodePro.Core.MVVM;

namespace NodePro.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const string TitleOrigin = "节点器";
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        private string _title = TitleOrigin;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand LoadedCommand { get; private set; }
        public MainWindowViewModel(IContainerProvider provider):base(provider)
        {
            _eventAggregator = provider.Resolve<IEventAggregator>();
            _regionManager = provider.Resolve<IRegionManager>();
            LoadedCommand = new DelegateCommand(ExecuteLoadedCommand);
            _eventAggregator.GetEvent<DirtyEvent>().Subscribe(OnDirtyEvent);

        }

        private void OnDirtyEvent(bool isDirty)
        {
            Title = TitleOrigin + (isDirty ? "(修改)" : string.Empty);
        }

        private void ExecuteLoadedCommand()
        {
            _regionManager.RequestNavigate(Regions.MainRegion, "DisplayView");
        }
    }
}
