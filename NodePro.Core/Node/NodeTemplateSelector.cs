using NodePro.Core.Model;
using System.Windows;
using System.Windows.Controls;

namespace NodePro.Core.Node
{
    public class NodeTemplateSelector:DataTemplateSelector
    {
        public Func<NodeElement,DependencyObject, DataTemplate?>? TemplateSelected { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is not NodeElement key) return base.SelectTemplate(item, container);
            DataTemplate? template = TemplateSelected?.Invoke(key, container);
            return template?? base.SelectTemplate(item, container);
        }
    }
}
