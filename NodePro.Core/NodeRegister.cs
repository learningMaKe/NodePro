using NodePro.Abstractions;
using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Interfaces;
using NodePro.Abstractions.Models;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using static NodePro.Abstractions.Constants.NodeRegisterConstants;

namespace NodePro.Core
{
    public class NodeRegister
    {
        private readonly NodeRegisterConfig? _config;
        private readonly Dictionary<string, HashSet<Type>> _scannedTypes = [];
        private readonly List<NodeRegisterKey> _registerKeys = [];
        private readonly Dictionary<string, NodeRegisterParams> _registerParameters = [];


        public Assembly[] DllGroup { get; set; } = [];
        public NodeRegister(string path)
        {
            _config = LoadConfig(path);
            DllGroup = ReadConfig(_config);
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

        #region Config Operation

        protected static Assembly[] ReadConfig(NodeRegisterConfig config)
        {
            // 验证配置和DLL列表
            if (config?.DllGroup == null || config.DllGroup.Count == 0)
                return [];


            HashSet<Assembly> assemblies = [];
            HashSet<string> assemblyKeys = [];

            bool AddAssemblyIfNotExists(Assembly assembly)
            {
                string? fullName = assembly.FullName;
                if (string.IsNullOrEmpty(fullName)) return false;
                if (!assemblyKeys.Add(fullName)) return false;
                return assemblies.Add(assembly);
            }
            // 处理配置文件中的DLL
            foreach (string dllPath in config.DllGroup.Where(IsValidAssembly))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dllPath);
                    AddAssemblyIfNotExists(assembly);
                }
                catch (Exception ex)
                {
                    // 记录加载失败的异常信息
                    Debug.WriteLine($"加载程序集 {dllPath} 失败: {ex.Message}");
                }
            }


            // 添加调用方和当前程序集（并检查是否已存在）
            AddAssemblyIfNotExists(Assembly.GetCallingAssembly());
            AddAssemblyIfNotExists(Assembly.GetExecutingAssembly());

            return assemblies.ToArray();
        }

        protected static NodeRegisterConfig LoadConfig(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return new NodeRegisterConfig();
            NodeRegisterConfig? res = null;
            if (File.Exists(path))
            {
                res = LoadConfigByFile(path);
            }
            else if (Directory.Exists(path))
            {
                res = LoadConfigByDir(path);
            }
            res ??= new NodeRegisterConfig();
            return res;
        }

        protected static NodeRegisterConfig? LoadConfigByDir(string path)
        {
            DirectoryInfo dir = new(path);
            if (!dir.Exists) return null;
            List<string> files = [.. dir.GetFiles().Where(x => IsValidAssembly(x.Name)).Select(x => x.FullName)];

            NodeRegisterConfig config = new()
            {
                DllGroup = files
            };

            return config;
        }

        protected static NodeRegisterConfig? LoadConfigByFile(string path)
        {
            NodeRegisterConfig? config = null;
            string? dir = Path.GetDirectoryName(path);
            if (string.IsNullOrWhiteSpace(dir))
            {
                throw new ArgumentException($"Invalid Path");
            }
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(path))
            {
                using FileStream fs = new(path, FileMode.Create);
                using StreamWriter writer = new(fs);
                config = new NodeRegisterConfig();
                string xml = SerializeConfig(config);
                writer.Write(xml);
            }
            else
            {
                using StreamReader reader = new(path);
                string xml = reader.ReadToEnd();
                config = DeserializeConfig(xml);
            }
            return config;
        }

        protected static string SerializeConfig(NodeRegisterConfig config)
        {
            if (config is null) return string.Empty;
            var serializer = new XmlSerializer(typeof(NodeRegisterConfig));
            using var sw = new StringWriter();
            serializer.Serialize(sw, config);
            return sw.ToString();
        }

        protected static NodeRegisterConfig? DeserializeConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return null;

            // 创建XmlSerializer实例
            var serializer = new XmlSerializer(typeof(NodeRegisterConfig));

            // 使用StringReader读取XML内容并反序列化
            using var reader = new StringReader(xml);
            return (NodeRegisterConfig?)serializer.Deserialize(reader);
        }

        #endregion

        #region Private Methods

        private void ScanType(Type type, params NodeRegisterKey[] validKeys)
        {
            if (!type.IsClass || type.IsAbstract) return;
            NodeRegisterAttribute? register = type.GetCustomAttribute<NodeRegisterAttribute>();
            if(register == null) return;
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
                    NodeRegisteredData data = new(type,register.GetExtraData());
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

    }

    public static class NodeRegisters
    {

        private static readonly Lazy<NodeRegister> _defaultRegister = new Lazy<NodeRegister>(CreateDefaultRegister);
        public static NodeRegister DefaultRegister => _defaultRegister.Value;


        private static NodeRegister CreateDefaultRegister()
        {
            string serviceKey = NodeRegisterConstants.Services;
            string nodeKey = NodeRegisterConstants.Nodes;
            string lineKey = NodeRegisterConstants.Lines;

            var ScanService = new CommonNodeRegisterKey(serviceKey, NodeRegisterType.Singleton);
            var ScanLine = new CommonNodeRegisterKey(lineKey, NodeRegisterType.Singleton);
            var ScanNode = new CommonNodeRegisterKey(nodeKey, NodeRegisterType.Instance);


            NodeRegister register = NodeRegister.Combine(ConfigPath, DllDirPath);
            register.
                AddKey(ScanNode).
                AddKey(ScanService).
                AddKey(ScanLine).
                Scan();
            return register;
        }
    }


}
