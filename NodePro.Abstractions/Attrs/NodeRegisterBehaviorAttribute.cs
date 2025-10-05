using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Attrs
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class NodeRegisterBehaviorAttribute:NodeBehaviorAttribute
    {
        public NodeRegisterBehaviorAttribute(string tag,Type handlerType):base(NodeConstants.BehaviorRegister,tag,handlerType)
        {

        }

        protected override Type BaseBehavior => typeof(IRegisterBehavior);

    }
}
