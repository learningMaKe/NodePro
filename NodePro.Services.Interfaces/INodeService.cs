using NodePro.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodePro.Services.Interfaces
{
    public interface INodeService
    {
        public void RegisterTemplate(string key, DataTemplate template);

        public void RegisterSheet<TSheet>(string key) where TSheet : NodeSheet;

        public void ProvideNode<TSheet>() where TSheet : NodeSheet;


    }
}
