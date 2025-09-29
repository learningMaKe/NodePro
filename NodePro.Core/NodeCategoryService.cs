using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core
{


    [NodeService]
    public class NodeCategoryService
    {
        private class CategoryItem : Dictionary<string, Type>
        {

            public string Content = string.Empty;

            public CategoryItem(string content)
            {
                Content = content;
            }
        }

        private List<KeyValuePair<string, Type>> _nodeMap = [];

        private CategoryItem Root { get; set; } = new CategoryItem("Root");
        public NodeCategoryService(INodeRegister register)
        {
            Type[] types = register.GetRegisterTypes(NodeConstants.KeyNodes);
            foreach(var type in types)
            {
                string category=type.Name;
                NodeCategoryAttribute? attribute = type.GetCustomAttribute<NodeCategoryAttribute>();
                if (attribute != null)
                {
                    category = attribute.Category;
                }
                CategoryItem item = ParseCategory(category);
                

            }
        }
        //ToDo: 解析Category
        private CategoryItem ParseCategory(string category)
        {
            string[] items = category.Split('\\', '/');
            throw new NotImplementedException();

        }


    }
}
