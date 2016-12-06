using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using TestInterfaces;
using TestImplementations;
using Nicolai.Utils.Composition.CompositionHelper;
using NSubstitute;

namespace UnitTests
{
    [TestClass]
    public class SimpleUsageTests
    {

        private Assembly interfaceAssembly;
        private Assembly implementationAssembly;

        [TestInitialize]
        public void InitializeAssemblies()
        {
            this.interfaceAssembly = Assembly.GetAssembly(typeof(Interface1));
            this.implementationAssembly = Assembly.GetAssembly(typeof(Implementation1));
        }

        [TestMethod]
        public void GetExport_StandardImpl_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { this.interfaceAssembly, this.implementationAssembly });

            // Act
            var instance = helper.GetExport<Interface1>();

            //Assert
            Assert.IsNotNull(instance);
            Assert.AreEqual(instance.GetInnerMessage(), "Hello from " + typeof(Implementation2).Name);
        }

        [TestMethod]
        public void GetExport_WrongStandardImpl_Fail()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { this.interfaceAssembly, this.implementationAssembly });

            // Act
            var instance = helper.GetExport<Interface1>();

            //Assert
            Assert.IsNotNull(instance);
            Assert.AreNotEqual(instance.GetInnerMessage(), "Good bye from " + typeof(Implementation2).Name);
        }

        [TestMethod]
        public void GetExport_StubImpl_Success()
        {
            // Arrange
            string stubResponse = "Hello from stub";
            var stubImplementation2 = Substitute.For<Interface2>();
            stubImplementation2.GetMessage().Returns(stubResponse);
            var helper = new CompositionHelper().AddAssemblies(new[] { this.interfaceAssembly, this.implementationAssembly }, typeof(Implementation2));
            helper.ComposeExport(stubImplementation2);

            // Act
            var instance = helper.GetExport<Interface1>();

            //Assert
            Assert.IsNotNull(instance);
            Assert.AreEqual(instance.GetInnerMessage(), stubResponse);
        }
    }
}
