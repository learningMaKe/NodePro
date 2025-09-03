using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core
{
    public class NValue<T> 
    {

        private T? _value = default;
        public T? Value
        {
            get { return _value; }
            set 
            { 
                _value = value;
                ValueChanged?.Invoke(_value);
            }
        }

        public Action<T?>? ValueChanged;
    }
}
