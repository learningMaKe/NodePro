using NodePro.Core.Attrs;
using NodePro.Core.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core
{
    public abstract class NodeSheet
    {

        protected IContainerProvider _containerProvider;
        protected NodeCreator _creator;

        private static readonly Dictionary<Type, (PropertyInfo, NodePropertyAttribute)[]> _reflectionProperties = [];
        public abstract string Title { get; }

        protected NodeSheet(IContainerProvider containerProvider) 
        {
            _containerProvider = containerProvider;
            _creator = containerProvider.Resolve<NodeCreator>();
        }

        public abstract NodeData DataProcess(NodeData data);

        public NodeModel CreateModel()
        {
            NodeModel model = new NodeModel()
            {
                Title = Title,
                Elements = GetNodeElements(),
            };

            return model;
        }

        private NodeElementGroup GetNodeElements()
        {
            (PropertyInfo, NodePropertyAttribute)[] attributes = GetAttributes();
            NodeElementGroup elements = new(attributes.Select(x => new NodeElement()
            {
                Template = x.Item2.Template,
                Content = x.Item1.GetValue(this),
                Sheet = this,
                Prop = x.Item1
            }).ToArray());
            return elements;
        }

        public (PropertyInfo, NodePropertyAttribute)[] GetAttributes()
        {
            Type nodeType = this.GetType();
            (PropertyInfo, NodePropertyAttribute)[]? attributes = [];
            try
            {
                //attributes = _reflectionProperties.GetOrAdd(nodeType, type =>
                //    [.. type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(prop=>prop.get)
                //.Select(prop => (prop, prop.GetCustomAttribute<NodePropertyAttribute>()))]);
                
                if(!_reflectionProperties.TryGetValue(nodeType,out attributes))
                {
                    PropertyInfo[] props = nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    List<(PropertyInfo, NodePropertyAttribute)> targets = [];
                    foreach (var prop in props)
                    {
                        NodePropertyAttribute? nodeProp = prop.GetCustomAttribute<NodePropertyAttribute>();
                        if (nodeProp is null) continue;
                        targets.Add((prop, nodeProp));
                    }
                    attributes = targets.ToArray();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nodeType.FullName}获取属性出错:{ex.Message}");
            }
            return attributes!;
        }
    }

    [Node]
    public class DemoSheet : NodeSheet
    {
        public override string Title => "测试节点";

        [NodeProperty("IntValue")]
        public string Value { get; set; } = "测试";

        [NodeProperty("StringValue")]
        public string Name { get; set; } = "你也要测试吗";

        public DemoSheet(IContainerProvider provider) : base(provider)
        {

        }

        public override NodeData DataProcess(NodeData data)
        {
            throw new NotImplementedException();
        }
    }
}
