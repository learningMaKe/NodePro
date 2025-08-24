using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Model
{
    public class NodeElementGroup : ObservableCollection<NodeElement>
    {

    }

    public class NodeElement:BindableBase
    {

        private object? _content = null;

        public object? Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        private string _template = string.Empty;

        public string Template
        {
            get { return _template; }
            set { SetProperty(ref _template, value); }
        }
    }
}
