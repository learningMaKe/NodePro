using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NodePro.Abstractions.Models
{
    public class NodeRegisterConfig
    {

        [XmlArray("DllGroup"), XmlArrayItem("Dll")]
        public List<string> DllGroup = [];
    }
}
