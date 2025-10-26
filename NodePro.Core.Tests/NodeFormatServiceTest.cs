using Moq;
using NodePro.Abstractions.Constants;
using NodePro.Abstractions.Interfaces;
using NodePro.Core.Formats;
using NodePro.Sheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Tests
{
    public class NodeFormatServiceTest
    {
        [Theory]
        [InlineData("Hello")]
        [InlineData("World")]
        public void TestDemo(string valueA)
        {
            INodeFormatService service = CreateFormatService();
            if(service.Format(valueA, "Demo1", "Demo2") is string valueB)
            {
                Assert.Equal(valueA + "World", valueB);
            }
        }


        [Theory]
        [InlineData(30)]
        [InlineData(70)]
        public void TestIntString(int valueA)
        {
            INodeFormatService service = CreateFormatService();
            if (service.Format(valueA, NodeFormats.Int,NodeFormats.String) is string valueB)
            {
                Assert.Equal(valueA.ToString(), valueB);
            }
           
        }

        private INodeFormatService CreateFormatService()
        {
            Type[] types = [
                typeof(IntStringFormatter),
                typeof(DemoFormatter),
                ];
            var mockRegister = new Mock<INodeRegister>();
            mockRegister.
                Setup(r => r.GetRegisterTypes(NodeConstants.KeyFormat)).
                Returns(types);
            var mockProvider = new Mock<IProvider>();
            mockProvider.Setup(p => p.Resolve(typeof(IntStringFormatter))).Returns(new IntStringFormatter());
            mockProvider.Setup(p=>p.Resolve(typeof(DemoFormatter))).Returns(new DemoFormatter());
            var service = new NodeFormatService(mockRegister.Object,mockProvider.Object);
            return service;
        }
    }
}
