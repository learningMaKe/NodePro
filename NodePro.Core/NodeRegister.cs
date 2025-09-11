using NodePro.Core.Attrs;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Resolvers;
using System.Xml.Serialization;

namespace NodePro.Core
{
    public class NodeConfig
    {

        [XmlArray("DllGroup"),XmlArrayItem("Dll")]
        public List<string> DllGroup = [];
    }

    public class NodeRegisterPath
    {
        public const string ConfigPath = "Config\\Node.Config";
        public const string DllDirPath = "Dll\\";
    }

    public class NodeRegisterKey
    {
        public const string Services = "Services";
        public const string Nodes = "Nodes";

        public required string Key { get; set; }

        public Action<Type>? Selected { get; set; }
        public required Func<Type, bool> Filter { get; set; }

        public NodeRegisterKey()
        {
                
        }
    }

    public class NodeRegister
    {

        private readonly NodeConfig? _config;
        private readonly Dictionary<string, List<Type>> _scannedTypes = [];
        private readonly List<NodeRegisterKey> _registerKeys = [];


        public Assembly[] DllGroup { get; set; }
 
        public NodeRegister(string path)
        {
            _config = LoadConfig(path);
            DllGroup = ReadConfig(_config);
        }

        private NodeRegister(Assembly[] assemblies)
        {
            DllGroup = assemblies;
        }


        public static NodeRegister Combine(params string[] paths)
        {
            NodeRegister[] registers = paths.Select(p => new NodeRegister(p)).ToArray();
            return Combine(registers);
        }

        public static NodeRegister Combine(params NodeRegister[] registers)
        {
            List<Assembly> assemblies = registers.Select(x => x.DllGroup).SelectMany(x => x).ToList();
            return new NodeRegister(assemblies.ToArray());
        }

        public List<Type> GetScannedTypes(string key)
        {
            if (_scannedTypes.TryGetValue(key, out List<Type>? types)) return types;
            return [];
        }

        public NodeRegister AddKey(NodeRegisterKey key)
        {
            _registerKeys.Add(key);
            return this;
        }

        public NodeRegister Scan()
        {
            Type[] typesToScan = DllGroup.Select(x => x.GetTypes()).SelectMany(x => x).ToArray() ?? [];
            NodeRegisterKey[] vaildKeys = _registerKeys.Where(x => !_scannedTypes.ContainsKey(x.Key)).ToArray();
            foreach(var key in vaildKeys)
            {
                _scannedTypes.TryAdd(key.Key, []);
            }
            foreach(var type in typesToScan)
            {
                if (!type.IsClass || type.IsAbstract) continue;
                foreach (var key in vaildKeys)
                {
                    if (key.Filter?.Invoke(type) == false) continue;
                    var typeGroup = _scannedTypes.GetValueOrDefault(key.Key);
                    if (typeGroup == null) continue;
                    typeGroup.Add(type);
                    key.Selected?.Invoke(type);
                }
            }
            _registerKeys.Clear();
            return this;
        }



        private static string SerializeConfig(NodeConfig config)
        {
            if (config is null) return string.Empty;
            var serializer = new XmlSerializer(typeof(NodeConfig));
            using var sw = new StringWriter();
            serializer.Serialize(sw, config);
            return sw.ToString();
        }

        private static NodeConfig? DeserializeConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return null;

            // 创建XmlSerializer实例
            var serializer = new XmlSerializer(typeof(NodeConfig));

            // 使用StringReader读取XML内容并反序列化
            using var reader = new StringReader(xml);
            return (NodeConfig?)serializer.Deserialize(reader);
        }

        private static bool IsValidAssembly(string path)
        {
            return !string.IsNullOrEmpty(path)
                   && File.Exists(path)
                   && string.Equals(Path.GetExtension(path), ".dll", StringComparison.OrdinalIgnoreCase);
        }

        private static Assembly[] ReadConfig(NodeConfig config)
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

        private static NodeConfig LoadConfig(string path)
        {
            if(string.IsNullOrWhiteSpace(path)) return new NodeConfig();
            NodeConfig? res = null;
            if (File.Exists(path))
            {
                res = LoadConfigByFile(path);
            }
            else if (Directory.Exists(path))
            {
                res = LoadConfigByDir(path);
            }
            res ??= new NodeConfig();
            return res;
        }

        private static NodeConfig? LoadConfigByDir(string path) 
        {
            DirectoryInfo dir = new(path);
            if(!dir.Exists) return null;
            List<string> files = [.. dir.GetFiles().Where(x => IsValidAssembly(x.Name)).Select(x => x.FullName)];

            NodeConfig config = new()
            {
                DllGroup = files
            };

            return config;
        }

        private static NodeConfig? LoadConfigByFile(string path)
        {
            NodeConfig? config = null;
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
                config = new NodeConfig();
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

    }

    public static class NodeConstants
    {
        public static readonly NodeRegister DefaultRegister;

        public static readonly NodeRegisterKey ScanService;

        public static readonly NodeRegisterKey ScanNode;

        static NodeConstants()
        {
            ScanService = new NodeRegisterKey()
            { 
                Key=NodeRegisterKey.Services,
                Filter = x=> x.GetCustomAttribute<NodeServiceAttribute>() != null
            };
            ScanNode = new NodeRegisterKey()
            {
                Key = NodeRegisterKey.Nodes,
                Filter = x => x.GetCustomAttribute<NodeAttribute>() != null
            };
            DefaultRegister = NodeRegister.Combine(NodeRegisterPath.ConfigPath, NodeRegisterPath.DllDirPath);
            DefaultRegister.AddKey(ScanNode).AddKey(ScanService).Scan();

        }
    }
}
