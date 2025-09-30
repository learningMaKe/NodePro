using NodePro.Abstractions.Constants;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = NodeConstants.PathConfig, Watch = true)]
namespace NodePro.Core
{

    internal enum LogLevel
    {
        Info,
        Warn,
        Error

    }


    internal struct OctLogStruct
    {
        public DateTime Time;
        public LogLevel Level;
        public string Message;
        public string Source;
        public Exception exception;
    }

    public class NodeLogger
    {
        private static readonly BlockingCollection<OctLogStruct> buff = new BlockingCollection<OctLogStruct>();

        public static readonly log4net.ILog devicelog = log4net.LogManager.GetLogger("loginfo");

        static NodeLogger()
        {
            CommonUtils.CheckFile(NodeConstants.PathLog, NodeConstants.PathEmbedderLog);
            StartWork();
        }

        public static void StartWork()
        {
            Task.Run(DoWork);
        }

        private static void DoWork()
        {
            OctLogStruct buffHeader = new OctLogStruct()
            {
                Message = $"===== Node Log({DateTime.Now:G}) =====",
                Level = LogLevel.Info,
            };
            buff.Add(buffHeader);
            foreach (var octLogStruct in buff.GetConsumingEnumerable())
            {
                if (octLogStruct.Level == LogLevel.Info)
                {
                    if (devicelog.IsInfoEnabled)
                    {
                        devicelog.Info(octLogStruct.Message);
                    }
                }
                else if (octLogStruct.Level == LogLevel.Warn)
                {
                    if (devicelog.IsWarnEnabled)
                    {
                        devicelog.Warn(octLogStruct.Message);
                    }
                }
                else if (octLogStruct.Level == LogLevel.Error)
                {
                    if (devicelog.IsErrorEnabled)
                    {
                        if (octLogStruct.exception != null)
                        {
                            devicelog.Error(octLogStruct.Message, octLogStruct.exception);
                        }
                        else
                        {
                            devicelog.Error(octLogStruct.Message);
                        }
                    }
                }
            }
        }

        public static void DevInfo(string info, [CallerMemberName]string? caller=null)
        {
            OctLogStruct log = new OctLogStruct();
            log.Level = LogLevel.Info;
            log.Message = $"[{caller}]" + info;
            //log.Time = DateTime.Now;

            //buff.Enqueue(log);
            buff.Add(log);
        }

        public static void DevWarn(string warning ,[CallerMemberName] string? caller = null)
        {
            OctLogStruct log = new OctLogStruct();
            log.Level = LogLevel.Warn;
            log.Message = $"[{caller}]" + warning;
            //log.Time = DateTime.Now;

            //buff.Enqueue(log);
            buff.Add(log);
        }

        public static void DevError(string errMsg, [CallerMemberName] string? caller = null)
        {
            OctLogStruct log = new OctLogStruct();
            log.Level = LogLevel.Error;
            log.Message = $"[{caller}]" + errMsg;
            //log.Time = DateTime.Now;

            //buff.Enqueue(log);
            buff.Add(log);
        }


        public static void Close()
        {
            buff.CompleteAdding();
        }
    }
}
