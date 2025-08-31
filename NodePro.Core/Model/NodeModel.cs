using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Model
{
    public class NodeModel:BindableBase
    {
        #region Observable Properties

        private string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private NodeElementGroup _elements = [];
        public NodeElementGroup Elements
        {
            get { return _elements; }
            set { SetProperty(ref _elements, value); }
        }

        #endregion

        #region Commands


        #endregion


    }
}
