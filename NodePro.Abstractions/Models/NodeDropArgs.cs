using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Models
{
    public class NodeDropArgs:EventArgs
    {
        public INodeConnector? StartFrom;

        public INodeConnector? LeaveFrom;
    }
}
