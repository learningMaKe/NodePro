using Moq;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Interfaces;
using NodePro.Sheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Tests
{
    public class NodeCategoryServiceTests
    {
        [Fact]
        public void TestCategory()
        {
            Type[] types = [
                typeof(DemoSheet),
                typeof(Demo2Sheet)
                ];
            var mockRegister = new Mock<INodeRegister>();
            mockRegister.
                Setup(r => r.GetRegisterTypes(NodeConstants.KeyNodes)).
                Returns(types);
            var nodeCategoryService = new NodeCategoryService(mockRegister.Object);
            
            
        }
    }
}
