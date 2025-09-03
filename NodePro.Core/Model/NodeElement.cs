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
        public NodeElementGroup(params NodeElement[] elements)
        {
            AddRange(elements);
        }

        public void AddRange(IEnumerable<NodeElement> elements)
        {
            foreach(var element in elements) 
            {
                this.Add(element); 
            }
        }
    }

    public class NodeElement:BindableBase
    {

        private object? _content = null;

        public object? Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        private string _template = TemplateKey.DefaultNodeTemplate;

        public string Template
        {
            get { return _template; }
            set { SetProperty(ref _template, value); }
        }

        private string _format = string.Empty;
        public string Format
        {
            get { return _format; }
            set { SetProperty(ref _format, value); }
        }
    }
}
