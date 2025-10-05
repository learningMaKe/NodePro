using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Models;
using NodePro.Core.Registers;

[assembly:NodeConfig(NodeConstants.PathDllDir)]
[assembly:NodeConfig(NodeConstants.PathConfig)]

[assembly:NodeScanBehavior(NodeConstants.KeyTypeCommon,typeof(CommonScanBehavior))]

[assembly:NodeRegisterBehavior(NodeConstants.HandlerInstance,typeof(InstanceRegisterBehavior))]
[assembly:NodeRegisterBehavior(NodeConstants.HandlerSingleton,typeof(SingletonRegisterBehavior))]

[assembly:NodeKey(NodeConstants.KeyLines,NodeConstants.HandlerSingleton,NodeConstants.KeyTypeCommon)]
[assembly:NodeKey(NodeConstants.KeyNodes,NodeConstants.HandlerInstance,NodeConstants.KeyTypeCommon)]
[assembly:NodeKey(NodeConstants.KeyServices, NodeConstants.HandlerSingleton, NodeConstants.KeyTypeCommon)]