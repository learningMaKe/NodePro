using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Attrs
{
    /// <summary>
    /// 用于生成对应的<see cref="NodeRegisterKey"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly,AllowMultiple =true)]
    public class NodeKeyAttribute:Attribute
    {
        public string Tag = string.Empty;
        public string RegisterBehavior { get; set; } = string.Empty;

        public string ScanBehavior { get; set; }
        /// <summary>
        /// 用于生成NodeRegisterKey
        /// </summary>
        /// <param name="tag">NodeKey 唯一标识</param>
        /// <param name="registerBehavior">处理如何注册到容器的处理器Tag</param>
        /// <param name="scanBehavior">处理扫描到匹配类型时行为的处理器Tag</param>
        public NodeKeyAttribute(string tag, string registerBehavior = NodeConstants.HandlerSingleton, string scanBehavior=NodeConstants.KeyTypeCommon)
        {
            Tag = tag;
            RegisterBehavior = registerBehavior;
            ScanBehavior = scanBehavior;
        }
    }
}
