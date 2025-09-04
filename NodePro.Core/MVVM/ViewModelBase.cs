using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.MVVM
{
    public class ViewModelBase:BindableBase
    {
        protected readonly IContainerProvider _containerProvider;
        public ViewModelBase(IContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }
    }
}
