using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Exceptions;
using NodePro.Abstractions.Interfaces;
using NodePro.Abstractions.Models;
using NodePro.Core.Model;
using NodePro.Core.Node;
using Prism.Ioc;
using System.Windows;

namespace NodePro.Core
{

    public class NodeInitArgs : EventArgs
    {
        public Point Position { get; init; } = new Point();

    }

    [NodeService]
    public class NodeCreateService
    {
        private readonly IContainerProvider _provider;

        public NodeCreateService(IContainerProvider provider)
        {
            _provider = provider;
        }

        public static NodeElement CreateElement(INodeSheet sheet,NodeProperty property)
        {
            var element = new NodeElement()
            {
                Template = property.NodePropertyAttribute.Template,
                Content = property.Property.GetValue(sheet),
                Sheet = sheet,
                Prop = property.Property,
                Order = property.NodeOrderAttribute?.Order ?? 0,
                Mode = property.NodePropertyAttribute.Mode

            };
            return element;
        }

        public NodeElementGroup CreateElementGroup<TSheet>(TSheet? sheet = null) where TSheet : class, INodeSheet
        {
            if (sheet is null)
            {
                if (!_provider.IsRegistered<TSheet>())
                {
                    NodeMissingException.Throw<TSheet>();
                }
                sheet ??= _provider.Resolve<TSheet>();
            }
            var group = new NodeElementGroup();
            NodeProperty[] props = sheet.GetAttributes();
            foreach (var prop in props)
            {
                NodeElement element = CreateElement(sheet, prop);
                group.Add(element);
            }
            group = group.Resort();
            return group;
        }

        public NodeContainer CreateContainer<TSheet>(NodeInitArgs? args = null) where TSheet : NodeSheet
        {
            if (!_provider.IsRegistered<TSheet>())
            {
                NodeMissingException.Throw<TSheet>();
            }
            var sheet = _provider.Resolve<TSheet>();
            args ??= new NodeInitArgs();
            NodeElementGroup group=CreateElementGroup(sheet);
            NodeContainer container = new NodeContainer()
            {
                Position = args.Position,
                Header = sheet.Title,
                Elements = group,
            };
            
            return container;
        }

        public NodeContainer? CreateContainer(Type sheetType,NodeInitArgs? args = null)
        {
            var sheet = _provider.Resolve(sheetType) as NodeSheet;
            if (sheet is null) 
            {
                NodeMissingException.Throw(sheetType);
                return null;
            }
            args ??= new NodeInitArgs();
            NodeElementGroup group = CreateElementGroup(sheet);
            NodeContainer container = new NodeContainer()
            {
                Position = args.Position,
                Header = sheet.Title,
                Elements = group,
            };
            return container;

        }
    }
}
