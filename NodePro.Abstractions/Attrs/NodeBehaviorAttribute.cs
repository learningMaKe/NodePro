using Microsoft.Xaml.Behaviors;
using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Attrs
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public abstract class NodeBehaviorAttribute : Attribute
    {
        public string GroupKey;

        public string Key;

        public Type Behavior;

        protected abstract Type BaseBehavior { get; }
        protected NodeBehaviorAttribute(string groupKey, string key, Type behavior)
        {
            GroupKey = groupKey;
            Key = key;
            Behavior = behavior;
        }

        public Dictionary<string, Type> Handle(Assembly assembly) 
        {
            Dictionary<string, Type> behaviors = [];
            IEnumerable<NodeBehaviorAttribute> attributes = assembly.GetCustomAttributes(this.GetType()).Cast<NodeBehaviorAttribute>();
            foreach (NodeBehaviorAttribute attribute in attributes)
            {
                if (attribute.Behavior.IsAssignableTo(attribute.BaseBehavior))
                {
                    behaviors.Add(attribute.Key, attribute.Behavior);
                }
            }
            return behaviors;
        }

    }


}
