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

        /// <summary>
        /// 对应的是添加到容器的处理器Tag
        /// </summary>
        public string HadnlerTag { get; set; }

        

        public NodeRegisterAttribute(string tag,string handlerTag)
        {
            Tag = tag;
            HadnlerTag = handlerTag;
        }

        public virtual object? GetExtraData() 
        {
            return null;
        }



    }

}
