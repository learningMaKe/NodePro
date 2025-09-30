using NodePro.Abstractions.Interfaces;
using NodePro.Core;
using NodePro.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodePro.Services
{
    public class NodeService:INodeService
    {
        public NodeService()
        {

        }

        public void ProvideNode<TSheet>() where TSheet : INodeSheet
        {
            throw new NotImplementedException();
        }

        public void RegisterSheet<TSheet>(string key) where TSheet : INodeSheet
        {
            throw new NotImplementedException();
        }

        public void RegisterTemplate(string key, DataTemplate template)
        {
            throw new NotImplementedException();
        }
    }
}
