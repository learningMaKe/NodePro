using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Interfaces;
using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Constants
{
    public class NodeConstants
    {
        public const string PathLog = "Config\\Log4net.config";
        public const string PathConfig = "Config\\Node.Config";
        public const string PathDllDir = "Dll\\";
        public const string PathEmbeddedConfig = "pack://application:,,,/NodePro.Abstractions;component/Config/Node.Config";
        public const string PathEmbedderLog = "pack://application:,,,/NodePro.Abstractions;component/Config/Log4net.config";


        public const string KeyServices = "Services";
        public const string KeyNodes = "Nodes";
        public const string KeyLines = "Lines";

        public const string HandlerSingleton = "HandlerSingleton";
        public const string HandlerInstance = "HandlerInstance";

        public const string ParamsDrop = "ParamsDrop";


    }
}
