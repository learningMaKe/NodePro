using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Interfaces;
using NodePro.Abstractions.Models;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core
{

    public abstract class NodeSheet:INodeSheet
    {

        protected IContainerProvider _containerProvider;
        protected NodeCreateService _creator;

        private static readonly Dictionary<Type, NodeProperty[]> _reflectionProperties = [];
        public abstract string Title { get; }

        protected NodeSheet(IContainerProvider containerProvider) 
        {
            _containerProvider = containerProvider;
            _creator = containerProvider.Resolve<NodeCreateService>();
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
