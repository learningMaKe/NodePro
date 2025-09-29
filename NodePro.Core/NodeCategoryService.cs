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
        private class CategoryItem : Dictionary<string, CategoryItem>
        {

            public string Content = string.Empty;

            public Type? Target;

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
                ParseCategory(category, type);
            }
        }
        //ToDo: 解析Category
        private void ParseCategory(string category,Type target)
        {
            string[] contents = category.Split('\\', '/');
            CategoryItem current = Root;
            foreach (var content in contents) 
            {
                CategoryItem item = GetOrCreateItem(current, content);
                current = item;
            }
            current.Target = target;

        }

        private CategoryItem GetOrCreateItem(CategoryItem item, string content)
        {
            CategoryItem? next = item.GetValueOrDefault(content);
            if (next == null)
            {
                next = new CategoryItem(content);
                item.Add(content, next);
            }
            return next;
        }


    }
}
