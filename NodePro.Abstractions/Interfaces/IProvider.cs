using NodePro.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Abstractions.Interfaces
{
    public interface IProvider
    {
        public void Register(RegisterType type, Type target);

        public void Register(RegisterType type, Type from, Type to);

        public T Resolve<T>();


        public object Resolve(Type type);
    }
}
