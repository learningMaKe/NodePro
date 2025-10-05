using Microsoft.Win32;
using NodePro.Abstractions;
using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Interfaces;
using NodePro.Abstractions.Models;
using NodePro.Core.Node;
using NodePro.Core.Registers;
using Prism.Ioc;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace NodePro.Core
{
    
    public class NodeRegister:INodeRegister
    {
        private readonly IProvider _provider;
        private readonly Dictionary<string, HashSet<Type>> _scannedTypes = [];
        private readonly List<NodeRegisterKey> _registerKeys = [];
        private readonly ConcurrentDictionary<string, Func<IScanBehavior?>> _createScanBehaviors = [];
        private readonly ConcurrentDictionary<string, NodeRegisterParams> _registerParameters = [];
        private readonly ConcurrentDictionary<string, Func<IRegisterBehavior?>> _createRegisterBehaviors = [];
        private readonly NodeBehaviorData _behaviorsData = [];
        public RegisterState State { get;private set; } = RegisterState.NotScanned;

        public Assembly[] DllGroup { get; set; } = [];
        public NodeRegister(IProvider containerProvider)
        {
            _provider = containerProvider;
        }

        public void Ioc()
        {
            NodeRegisteredData[] registedDatas = GetRegisteredData();
            foreach (var data in registedDatas)
            {
                if (data.RegisterBehavior is null) continue;
                data.RegisterBehavior.OnRegister(data, _provider);
            }
        }

        public NodeRegister AddKey(NodeRegisterKey key)
        {
            if (_registerKeys.Any(x => x.Key == key.Key)) return this;
            _registerKeys.Add(key);
            return this;
        }

        public NodeRegisterParams GetParameters(string key)
        {
            if (!CheckState()) return [];
            if(_registerParameters.TryGetValue(key, out var parameters)) return parameters;
            return [];
        }

        public Type[] GetRegisterTypes(string key)
        {
            Type[] types = GetRegisteredData(key).Select(x => x.DataType).ToArray();
            return types;
        }


        public Type[] GetRegisterTypes()
        {
            return GetRegisteredData().Select(x  => x.DataType).ToArray();
        }

        public NodeRegisteredData[] GetRegisteredData()
        {
            List<NodeRegisteredData> datas = [];
            string[] keys = _registerParameters.Keys.ToArray();
            foreach (string key in keys) 
            {
                datas.AddRange(GetRegisteredData(key));
            }
            return datas.ToArray();
        }

        public NodeRegisteredData[] GetRegisteredData(string key)
        {
            var parameters = GetParameters(key);
            NodeRegisteredData[] datas = parameters.GetValues<NodeRegisteredData>(key).ToArray();
            return datas;
        }

        public RegisterState Scan()
        {
            if (State == RegisterState.Scanned) return State;
            if (State == RegisterState.Scaning) return State;
            State = RegisterState.Scaning;
            try
            {
                ScanAssemblyConfig();
                // 先扫属性，再扫类
                foreach (Assembly assembly in DllGroup)
                {
                    ScanAssemblyAttribute(assembly);
                }
                foreach (Assembly assembly in DllGroup)
                {
                    ScanAssemblyType(assembly);
                }

                State = RegisterState.Scanned;
            }
            catch (Exception ex)
            {
                NodeLogger.DevError(ex.Message);
                State = RegisterState.ScanError;
            }
            return State;
        }


        #region Private Methods
        private bool CheckState()
        {
            return State == RegisterState.Scanned;
        }

        private void ScanAssemblyAttribute(Assembly assembly)
        {
            ScanAssemblyScanBehavior(assembly);
            ScanAssemblyRegisterBehavior(assembly);
            ScanAssemblyKey(assembly);
        }

        private void ScanAssemblyConfig()
        {
            Assembly[] GetConfigs(Assembly assembly)
            {
                NodeRegisterConfig config = NodeConfigger.CombineConfig(assembly.GetCustomAttributes<NodeConfigAttribute>().Select(x => x.Path).ToArray());
                return NodeConfigger.ReadConfig(config);
            }
            string GetName(Assembly assembly)
            {
                return assembly.FullName ?? assembly.GetHashCode().ToString();
            }
            List<Assembly> assemblies = [];
            Stack<Assembly> stack = new Stack<Assembly>();
            HashSet<string> scanedAssembly = [];
            stack.Push(Assembly.GetExecutingAssembly());
            while (stack.Count > 0)
            {
                Assembly asm = stack.Pop();
                assemblies.Add(asm);
                scanedAssembly.Add(GetName(asm));
                Assembly[] others = GetConfigs(asm).Where(x=>!scanedAssembly.Contains(GetName(x))).ToArray();
                foreach (Assembly other in others)
                {
                    stack.Push(other);
                }
            }
            DllGroup = assemblies.DistinctBy(x => x.FullName).ToArray();
            
        }

        private void ScanAssemblyScanBehavior(Assembly assembly)
        {
            IEnumerable<NodeScanBehaviorAttribute> keyTypeAttributes = assembly.GetCustomAttributes<NodeScanBehaviorAttribute>();
            foreach (NodeScanBehaviorAttribute keyTypeAttribute in keyTypeAttributes)
            {
                if (string.IsNullOrEmpty(keyTypeAttribute.Key)) continue;
                Type handlerType = keyTypeAttribute.Behavior;
                if (handlerType.IsAbstract || handlerType.IsInterface) continue;
                if (!typeof(IScanBehavior).IsAssignableFrom(handlerType)) continue;
                _provider.Register(RegisterType.Singleton, handlerType);
                _createScanBehaviors.GetOrAdd(keyTypeAttribute.Key, () =>
                {
                    IScanBehavior? scanBehavior = _provider.Resolve(handlerType) as IScanBehavior;
                    return scanBehavior;
                });
            }
        }

        private void ScanAssemblyKey(Assembly assembly)
        {
            IEnumerable<NodeKeyAttribute> keyAttributes = assembly.GetCustomAttributes<NodeKeyAttribute>();
            foreach(var keyAttribute in keyAttributes)
            {
                IScanBehavior? scanBehavior = _createScanBehaviors.GetValueOrDefault(keyAttribute.ScanBehavior)?.Invoke();
                if (scanBehavior is null) continue;
                NodeRegisterKey key = new NodeRegisterKey()
                {
                    Key = keyAttribute.Tag,
                    ScanBehavior = scanBehavior,
                };
                _registerKeys.Add(key);
            }
        }
        private void ScanAssemblyRegisterBehavior(Assembly assembly)
        {
            IEnumerable<NodeRegisterBehaviorAttribute> handlerAttributes= assembly.GetCustomAttributes<NodeRegisterBehaviorAttribute>();
            foreach(var handlerAttribute in handlerAttributes)
            {

                Type handlerType = handlerAttribute.Behavior;
                if (!typeof(IRegisterBehavior).IsAssignableFrom(handlerType))
                {
                    throw new ArgumentException($"{assembly.FullName} Handler {handlerAttribute.Key} {handlerAttribute.Behavior.Name} is not assigned to {typeof(IRegisterBehavior).Name}");
                }
                _provider.Register(RegisterType.Singleton, handlerType);
                _createRegisterBehaviors.GetOrAdd(handlerAttribute.Key, () =>
                {
                    IRegisterBehavior? nodeRegisterTypeHandler = _provider.Resolve(handlerType) as IRegisterBehavior;
                    if (nodeRegisterTypeHandler is null) return null;
                    return nodeRegisterTypeHandler;
                });
            }
        }

        private void ScanAssemblyType(Assembly assembly)
        {
            Type[] typesToScan = assembly.GetTypes().ToArray() ?? [];
            NodeRegisterKey[] validKeys = _registerKeys.Where(x => !_scannedTypes.ContainsKey(x.Key)).ToArray();
            foreach (var key in validKeys)
            {
                _scannedTypes.TryAdd(key.Key, []);
                _registerParameters.GetOrAdd(key.Key, []);
            }
            foreach (var type in typesToScan)
            {
                ScanType(type, validKeys);
            }
        }

        private void ScanType(Type type, params NodeRegisterKey[] validKeys)
        {
            if (!type.IsClass || type.IsAbstract) return;
            NodeRegisterAttribute? register = type.GetCustomAttribute<NodeRegisterAttribute>();
            if(register == null) return;
            IRegisterBehavior? handler = _createRegisterBehaviors.GetValueOrDefault(register.HadnlerTag)?.Invoke();
            NodeRegisterFilterParams filterParams = new()
            {
                TypeToFilter = type,
                Tag = register.Tag
            };
            foreach (NodeRegisterKey key in validKeys)
            {
                if (key.ScanBehavior?.Filter(key, filterParams) == false) continue;
                var typeGroup = _scannedTypes.GetValueOrDefault(key.Key);
                if (typeGroup == null) continue;
                bool isSuccess = typeGroup.Add(type);
                if (isSuccess)
                {
                    NodeRegisteredData data = new()
                    {
                        DataType = type,
                        ExtraData = register.GetExtraData(),
                        RegisterBehavior = handler,
                    };
                    key.ScanBehavior?.Selected(key, data, _registerParameters[key.Key]);
                }
            }
        }

        #endregion

        protected static bool IsValidAssembly(string path)
        {
            return !string.IsNullOrEmpty(path)
                   && File.Exists(path)
                   && string.Equals(Path.GetExtension(path), ".dll", StringComparison.OrdinalIgnoreCase);
        }

    }

}
