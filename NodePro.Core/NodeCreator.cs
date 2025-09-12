using NodePro.Core.Attrs;
using NodePro.Core.Exceptions;
using NodePro.Core.Interfaces;
using NodePro.Core.Model;
using NodePro.Core.Node;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodePro.Core
{

    public class NodeInitArgs : EventArgs
    {
        public Point Position { get; init; } = new Point();

    }

    [NodeService]
    public class NodeCreator
    {
        private readonly IContainerProvider _provider;

        public NodeCreator(IContainerProvider provider)
        {
            _provider = provider;
        }

        public static NodeElement CreateElement(NodeSheet sheet,NodeProperty property)
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

        public NodeElementGroup CreateElementGroup<TSheet>(TSheet? sheet = null) where TSheet : NodeSheet
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


    }
}
