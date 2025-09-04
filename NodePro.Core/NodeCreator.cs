using NodePro.Core.Attrs;
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

        public NodeConnectEventHandler? NodeConnect;

    }
    public class NodeCreator
    {
        private readonly IContainerProvider _provider;

        public NodeCreator(IContainerProvider provider)
        {
            _provider = provider;
        }

        public NodeElement CreateElement(NodeSheet sheet, PropertyInfo prop, NodePropertyAttribute attribute)
        {
            var element = new NodeElement()
            {
                Template = attribute.Template,
                Content = prop.GetValue(sheet),
                Sheet = sheet,
                Prop = prop
            };
            return element;
        }

        public NodeElementGroup CreateElementGroup<TSheet>(TSheet? sheet = null) where TSheet : NodeSheet
        {
            if (sheet is null)
            {
                if (!_provider.IsRegistered<TSheet>())
                {
                    throw new InvalidOperationException($"未注册的节点类型:{typeof(TSheet).FullName}");
                }
                sheet ??= _provider.Resolve<TSheet>();
            }
            var group = new NodeElementGroup();
            (PropertyInfo, NodePropertyAttribute)[] props = sheet.GetAttributes();
            foreach (var prop in props)
            {
                NodeElement element = CreateElement(sheet, prop.Item1, prop.Item2);
                group.Add(element);
            }
            return group;
        }

        public NodeContainer CreateContainer<TSheet>(NodeInitArgs? args = null) where TSheet : NodeSheet
        {
            if (!_provider.IsRegistered<TSheet>())
            {
                throw new InvalidOperationException($"未注册的节点类型:{typeof(TSheet).FullName}");
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
            
            if(args.NodeConnect is not null)
            {
                container.NodeConnect += args.NodeConnect;
            }
            return container;
        }
    }
}
