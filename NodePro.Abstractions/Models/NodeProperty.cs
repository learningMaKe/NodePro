using NodePro.Abstractions.Attrs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Models
{

    public class NodeProperty()
    {
        public required PropertyInfo Property { get; init; }

        public required NodePropertyAttribute NodePropertyAttribute { get; init; }

        public NodeOrderAttribute? NodeOrderAttribute { get; init; }


    }
}
