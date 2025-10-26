using NodePro.Abstractions.Interfaces;
using NodePro.Core;
using NodePro.Core.Registers;
using NodePro.Services;
using NodePro.Services.Interfaces;
using NodePro.ViewModels;
using NodePro.Views;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Windows;
using Wpf.Ui;

namespace NodePro
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App:PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IContentDialogService, ContentDialogService>();
            containerRegistry.RegisterSingleton<INodeService, NodeService>();
            PrismProvider provider = new(Container, containerRegistry);
            NodeRegister nodeRegister = new NodeRegister(provider);
            nodeRegister.Scan();
            nodeRegister.Ioc();
            containerRegistry.RegisterInstance(typeof(INodeRegister), nodeRegister);
            containerRegistry.RegisterInstance(typeof(IProvider), provider);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<NodePro.Modules.Display.DisplayModule>();
        }

    }

}
