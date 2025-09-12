using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Exceptions
{
    public enum ConnectionErrorCode
    {
        输入点多条连线,
    }
    public class NodeConnectException:Exception
    {
        public ConnectionErrorCode ConnectionErrorCode;
        public NodeConnectException(ConnectionErrorCode code) : base($"连线异常:{code}")
        {
            ConnectionErrorCode = code;
        }


    }
}
