using NodePro.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Attrs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeRegisterAttribute:Attribute
    {
        // Tag 是用来对应 NodeRegisterKey的，想要添加额外属性应该在NodeRegisterKey上改
        public string Tag { get;set; }


        public NodeRegisterAttribute(string tag)
        {
            Tag = tag;
        }

        public virtual object? GetExtraData() 
        {
            return null;
        }

    }
}
