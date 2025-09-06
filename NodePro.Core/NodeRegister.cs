using NodePro.Core.Attrs;
using System;
using System.Collections.Generic;
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
    public class NodeRegister
    {

        private readonly NodeConfig? _config;

        private readonly List<Type> _servicesTypes = [];
        private readonly List<Type> _nodeTypes = [];


        public NodeRegister(string path)
        {
            _config = LoadConfig(path);
            var group = ReadConfig(_config);
            
            _servicesTypes = group.Item1;
            _nodeTypes = group.Item2;
        }

        public NodeRegister(List<Type> serviceType, List<Type> nodeType)
        {
            _servicesTypes = serviceType;
            _nodeTypes = nodeType;
        }

        public List<Type> ServiceType => _servicesTypes;

        public List<Type> NodeType => _nodeTypes;

        public static NodeRegister Combine(params string[] paths)
        {
            NodeRegister[] registers = paths.Select(p => new NodeRegister(p)).ToArray();
            return Combine(registers);
        }

        public static NodeRegister Combine(params NodeRegister[] registers)
        {
            List<Type> serviceType = [.. registers.Select(reg => reg.ServiceType).SelectMany(x => x)];
            List<Type> nodeType = [.. registers.Select(reg => reg.NodeType).SelectMany(x => x)];
            return new NodeRegister(serviceType,nodeType);
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

        private static (List<Type>, List<Type>) ReadConfig(NodeConfig config)
        {
            // 验证配置和DLL列表
            if (config?.DllGroup == null || config.DllGroup.Count == 0)
                return ([], []);


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

            // 筛选带有指定特性的类型
            List<Type> serviceTypes = [];
            List<Type> nodeTypes = [];

            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsDefined(typeof(NodeServiceAttribute), inherit: false))
                        {
                            serviceTypes.Add(type);
                        }
                        if (type.IsDefined(typeof(NodeAttribute), inherit: false))
                        {
                            nodeTypes.Add(type);
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // 处理类型加载异常
                    Debug.WriteLine($"解析程序集 {assembly.FullName} 中的类型失败: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"处理程序集 {assembly.FullName} 时出错: {ex.Message}");
                }
            }

            return (serviceTypes, nodeTypes);
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
}
