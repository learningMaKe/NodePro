using NodePro.Core.Attrs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Model
{
    public enum NodeMode
    {
        Input,
        Output
    }

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

        public NodeElementGroup Resort()
        {
            // 优化点1：用OrderByDescending替代OrderBy+Reverse，减少一次反转操作
            // 优化点2：直接构建排序后的序列，减少中间数组创建
            var sortedElements = this
                .Where(x => x.Mode == NodeMode.Output)
                .OrderByDescending(x => x.Order) // 等价于OrderBy(x => x.Order).Reverse()
                .Concat(this
                    .Where(x => x.Mode == NodeMode.Input)
                    .OrderByDescending(x => x.Order)
                );

            return new NodeElementGroup(sortedElements.ToArray());
        }
    }

    public class NodeElement:BindableBase
    {
        public required NodeSheet Sheet { get; set; }

        public required PropertyInfo Prop { get; init; }

        #region Observable Properties

        private object? _content = null;

        public object? Content
        {
            get { return _content; }
            set 
            { 
                SetProperty(ref _content, value);
                OnContentChanged();
            }
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

        private NodeMode _mode = NodeMode.Input;
        public NodeMode Mode
        {
            get { return _mode; }
            set { SetProperty(ref _mode, value); }
        }

        private int _order;
        public int Order
        {
            get { return _order; }
            set { SetProperty(ref _order, value); }
        }

        #endregion

        public Action<object?>? ContentChanged = null;

        private void OnContentChanged()
        {
            if (Sheet is null) return;
            Prop.SetValue(Sheet, _content);
            ContentChanged?.Invoke(_content);
        }
    }

    
}
