using NodePro.Abstractions.Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodePro.Core
{
    public static class NodeIniter
    {
        public static void InitEnvironment()
        {
            NodeLogger.InitEnvironment();
            NodeConfiger.InitEnvironment();
        }

        public static void CheckFile(string path,string embeddedPath)
        {
            string configPath = path;
            FileInfo configInfo = new FileInfo(configPath);
            if (configInfo.Directory is DirectoryInfo configDir)
            {
                if (!configDir.Exists)
                {
                    configDir.Create();
                }
                if (!configInfo.Exists)
                {
                    if (ReadEmbeddedContent(embeddedPath, out string content))
                    {
                        using var fs = configInfo.Create();
                        byte[] bytes = Encoding.UTF8.GetBytes(content);
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        public static void CheckDir(string dir)
        {
            DirectoryInfo dllDir = new DirectoryInfo(dir);
            if (!dllDir.Exists)
            {
                dllDir.Create();
            }
        }

        public static bool ReadEmbeddedContent(string path, out string content)
        {
            content = string.Empty;
            Uri uri = new Uri(path, UriKind.RelativeOrAbsolute);
            try
            {
                var resourceInfo = Application.GetResourceStream(uri);

                // 从流中读取内容（假设是文本文件，如配置文件）
                using StreamReader reader = new StreamReader(resourceInfo.Stream);
                content = reader.ReadToEnd();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
