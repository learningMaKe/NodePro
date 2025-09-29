using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace NodePro.Core
{
    public class NodeConfiger
    {
        public static void InitEnvironment()
        {
            NodeIniter.CheckDir(NodeConstants.PathDllDir);
            NodeIniter.CheckFile(NodeConstants.PathConfig, NodeConstants.PathEmbeddedConfig);
        }

        #region Config Operation


        public static Assembly[] ReadConfig(NodeRegisterConfig config)
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

        public static NodeRegisterConfig LoadConfig(string path)
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

        public static NodeRegisterConfig? LoadConfigByDir(string path)
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

        public static NodeRegisterConfig? LoadConfigByFile(string path)
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

        public static string SerializeConfig(NodeRegisterConfig config)
        {
            if (config is null) return string.Empty;
            var serializer = new XmlSerializer(typeof(NodeRegisterConfig));
            using var sw = new StringWriter();
            serializer.Serialize(sw, config);
            return sw.ToString();
        }

        public static NodeRegisterConfig? DeserializeConfig(string xml)
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

        public static bool IsValidAssembly(string path)
        {
            return !string.IsNullOrEmpty(path)
                   && File.Exists(path)
                   && string.Equals(Path.GetExtension(path), ".dll", StringComparison.OrdinalIgnoreCase);
        }

    }
}
