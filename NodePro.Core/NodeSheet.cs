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
    public class NodeProperty()
    {
        public required PropertyInfo Property { get; init; }

        public required NodePropertyAttribute NodePropertyAttribute { get; init; }

        public NodeOrderAttribute? NodeOrderAttribute { get; init; }

        
    }

    public abstract class NodeSheet
    {

        protected IContainerProvider _containerProvider;
        protected NodeCreator _creator;

        private static readonly Dictionary<Type, NodeProperty[]> _reflectionProperties = [];
        public abstract string Title { get; }

        protected NodeSheet(IContainerProvider containerProvider) 
        {
            _containerProvider = containerProvider;
            _creator = containerProvider.Resolve<NodeCreator>();
        }

        public abstract NodeData DataProcess(NodeData data);

        public NodeProperty[] GetAttributes()
        {
            Type nodeType = this.GetType();
            NodeProperty[]? attributes = [];
            try
            {
                //attributes = _reflectionProperties.GetOrAdd(nodeType, type =>
                //    [.. type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(prop=>prop.get)
                //.Select(prop => (prop, prop.GetCustomAttribute<NodePropertyAttribute>()))]);
                
                if(!_reflectionProperties.TryGetValue(nodeType,out attributes))
                {
                    PropertyInfo[] props = nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    List<NodeProperty> targets = [];
                    foreach (var prop in props)
                    {
                        NodePropertyAttribute? nodeProp = prop.GetCustomAttribute<NodePropertyAttribute>();
                        if (nodeProp is null) continue;
                        NodeOrderAttribute? nodeOrder = prop.GetCustomAttribute<NodeOrderAttribute>();
                        NodeProperty property = new()
                        {
                            Property = prop,
                            NodePropertyAttribute = nodeProp,
                            NodeOrderAttribute = nodeOrder
                        };
                        targets.Add(property);
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

}
