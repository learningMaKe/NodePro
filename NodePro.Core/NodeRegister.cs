using NodePro.Abstractions;
using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Interfaces;
using NodePro.Abstractions.Models;
using NodePro.Core.Registers;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace NodePro.Core
{
    public class NodeRegister:INodeRegister
    {
        private readonly NodeRegisterConfig? _config;
        private readonly Dictionary<string, HashSet<Type>> _scannedTypes = [];
        private readonly List<NodeRegisterKey> _registerKeys = [];
        private readonly Dictionary<string, NodeRegisterParams> _registerParameters = [];
        private readonly Dictionary<string, INodeRegisterTypeHandler> _handlers = [];

        public Assembly[] DllGroup { get; set; } = [];
        public NodeRegister(string path)
        {
            _config = NodeConfiger.LoadConfig(path);
            DllGroup = NodeConfiger.ReadConfig(_config);
        }

        internal NodeRegister()
        {
            
        }
        private NodeRegister(params Assembly[] assemblies)
        {
            DllGroup = assemblies;
        }

        public static NodeRegister Combine(params string[] paths)
        {
            NodeRegister[] registers = [.. paths.Select(p => new NodeRegister(p))];
            return Combine(registers);
        }

        public static NodeRegister Combine(params NodeRegister[] registers)
        {
            List<Assembly> assemblies = registers.Select(x => x.DllGroup).SelectMany(x => x).DistinctBy(x => x.FullName).ToList();
            return new NodeRegister(assemblies.ToArray());
        }

        public List<Type> GetScannedTypes(string key)
        {
            if (_scannedTypes.TryGetValue(key, out HashSet<Type>? types)) return types.ToList();
            return [];
        }

        public NodeRegister AddKey(NodeRegisterKey key)
        {
            if (_registerKeys.Any(x => x.Key == key.Key)) return this;
            _registerKeys.Add(key);
            return this;
        }

        public NodeRegisterParams GetParameters(string key)
        {
            if(_registerParameters.TryGetValue(key, out var parameters)) return parameters;
            return [];
        }

        public Type[] GetRegisterTypes(string key)
        {
            Type[] types = GetRegisteredData(key).Select(x => x.DataType).ToArray();
            return types;
        }

        public Type[] GetRegisterTypes(NodeRegisterType type)
        {
            return GetRegisteredData(type).Select(x => x.DataType).ToArray();
        }

        public NodeRegisteredData[] GetRegisteredData(NodeRegisterType nodeRegisterType)
        {
            List<NodeRegisteredData> datas = [];
            foreach(var key in _registerKeys)
            {
                if(key.RegisterType != nodeRegisterType) continue;
                datas.AddRange(GetRegisteredData(key.Key));
            }
            return datas.ToArray();
        }

        public NodeRegisteredData[] GetRegisteredData(string key)
        {
            var parameters = GetParameters(key);
            NodeRegisteredData[] datas = parameters.GetValues<NodeRegisteredData>(key).ToArray();
            return datas;
        }

        public NodeRegister Scan()
        {
            Type[] typesToScan = DllGroup.Select(x => x.GetTypes()).SelectMany(x => x).ToArray() ?? [];
            NodeRegisterKey[] validKeys = _registerKeys.Where(x => !_scannedTypes.ContainsKey(x.Key)).ToArray();
            foreach(var key in validKeys)
            {
                _scannedTypes.TryAdd(key.Key, []);
                _registerParameters.Add(key.Key, []);
            }
            foreach(var type in typesToScan)
            {
                ScanType(type, validKeys);
            }
            return this;
        }


        #region Private Methods

        private void ScanType(Type type, params NodeRegisterKey[] validKeys)
        {
            if (!type.IsClass || type.IsAbstract) return;
            NodeRegisterAttribute? register = type.GetCustomAttribute<NodeRegisterAttribute>();
            if(register == null) return;
            INodeRegisterTypeHandler? handler = null;
            NodeRegisterFlagAttribute? flag = register.GetType().GetCustomAttribute<NodeRegisterFlagAttribute>();
            if (flag != null)
            {
                handler = _handlers.GetValueOrDefault(flag.Handler);
            }
            NodeRegisterFilterParams filterParams = new()
            {
                TypeToFilter = type,
                Tag = register.Tag
            };
            foreach (var key in validKeys)
            {
                if (key.Filter?.Invoke(filterParams) == false) continue;
                var typeGroup = _scannedTypes.GetValueOrDefault(key.Key);
                if (typeGroup == null) continue;
                bool isSuccess = typeGroup.Add(type);
                if (isSuccess)
                {
                    NodeRegisteredData data = new()
                    {
                        DataType = type,
                        ExtraData = register.GetExtraData(),
                        Handler = handler,
                    };
                    key.Selected?.Invoke(data, _registerParameters[key.Key]);
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

        public NodeRegister AddHandler<THandler>(string key,THandler handler) where THandler : class, INodeRegisterTypeHandler
        {
            _handlers.TryAdd(key, handler);
            return this;
        }


    }

    public static class NodeRegisters
    {

        private static readonly Lazy<NodeRegister> _defaultRegister = new Lazy<NodeRegister>(CreateDefaultRegister);
        public static NodeRegister DefaultRegister => _defaultRegister.Value;


        private static NodeRegister CreateDefaultRegister()
        {
            string serviceKey = NodeConstants.KeyServices;
            string nodeKey = NodeConstants.KeyNodes;
            string lineKey = NodeConstants.KeyLines;

            var ScanService = new CommonNodeRegisterKey(serviceKey, NodeRegisterType.Singleton);
            var ScanLine = new CommonNodeRegisterKey(lineKey, NodeRegisterType.Singleton);
            var ScanNode = new CommonNodeRegisterKey(nodeKey, NodeRegisterType.Instance);

            return NodeRegisterBuilder.
                GetBuilder().
                AddConfig(NodeConstants.PathConfig).
                AddConfig(NodeConstants.PathDllDir).
                Completed().
                AddHandler<SingletonHandler>(NodeConstants.HandlerSingleton).
                AddHandler<InstanceHandler>(NodeConstants.HandlerInstance).
                Completed().
                AddKey(ScanService).
                AddKey(ScanLine).
                AddKey(ScanNode).
                Build();
        }
    }


}
