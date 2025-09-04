using NodePro.Core;
using NodePro.Core.Attrs;
using NodePro.Core.Model;
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

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IContentDialogService, ContentDialogService>();
            containerRegistry.RegisterSingleton<INodeService, NodeService>();
            RegisterNode(containerRegistry);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<NodePro.Modules.Display.DisplayModule>();
        }

        private void RegisterNode(IContainerRegistry containerRegistry)
        {
            RegisterNodeService(containerRegistry);
            List<Assembly> assemblies = RegisterNodeAssembly();
            List<Type> nodeToRegister = [];
            foreach (Assembly assembly in assemblies) 
            {
                Type[] types = [.. assembly.GetTypes().Where(x => x.GetCustomAttribute<NodeAttribute>() is not null)];
                nodeToRegister.AddRange(nodeToRegister);
            }
            List<Type> singleNodes = RegisterSingleNode();
            nodeToRegister.AddRange(singleNodes);
            foreach (Type type in nodeToRegister)
            {
                containerRegistry.Register(type);
            }
        }

        private void RegisterNodeService(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<NodeCreator>();
            containerRegistry.RegisterSingleton<NodeFormatter>();
        }

        private List<Assembly> RegisterNodeAssembly()
        {
            List<Assembly> types = [
                Assembly.GetExecutingAssembly(),
                ];
            return types;
        }

        private List<Type> RegisterSingleNode()
        {
            return [];
        }



    }

}
