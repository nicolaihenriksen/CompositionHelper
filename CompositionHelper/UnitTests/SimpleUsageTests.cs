using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nicolai.Utils.Composition.CompositionHelper;
using System.Composition.Hosting;
using System.Composition;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class SimpleUsageTests
    {

        [TestMethod]
        public void GetExport_StandardImpl_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly });

            // Act
            var instance = helper.GetExport<ICompositionTest>();

            //Assert
            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(CompositionTestImplementation).FullName, instance.GetType().FullName);
        }

        [TestMethod]
        public void TryGetExport_StandardImpl_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly });

            // Act
            ICompositionTest instance;
            bool result = helper.TryGetExport(out instance);

            //Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(CompositionTestImplementation).FullName, instance.GetType().FullName);
        }


        [TestMethod]
        public void GetNamedExport_StandardImpl_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly });

            // Act
            var instance = helper.GetExport<ICompositionTest>("NamedImplementation");

            //Assert
            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(NamedCompositionTestImplementation).FullName, instance.GetType().FullName);
        }

        [TestMethod]
        public void TryGetNamedExport_StandardImpl_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly });

            // Act
            ICompositionTest instance;
            bool result = helper.TryGetExport(out instance, "NamedImplementation");

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(NamedCompositionTestImplementation).FullName, instance.GetType().FullName);
        }

        [TestMethod]
        [ExpectedException(typeof(CompositionFailedException))]
        public void GetNamedExport_WrongContractName_Throws()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly });

            // Act and Assert
            helper.GetExport<ICompositionTest>("WrongName");
        }

        [TestMethod]
        public void TryGetNamedExport_WrongContractName_Fails()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly });

            // Act
            ICompositionTest instance;
            var result = helper.TryGetExport(out instance, "WrongName");

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(instance);
        }

        [TestMethod]
        public void GetExport_StubImpl_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly }, typeof(CompositionTestImplementation));
            ICompositionTest stub = new StubCompositionTestImplementation();
            helper.ComposeExport(stub);

            // Act
            var instance = helper.GetExport<ICompositionTest>();

            // Assert
            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(StubCompositionTestImplementation).FullName, instance.GetType().FullName);
        }

        [TestMethod]
        public void TryGetExport_StubImpl_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly }, typeof(CompositionTestImplementation));
            ICompositionTest stub = new StubCompositionTestImplementation();
            helper.ComposeExport(stub);

            // Act
            ICompositionTest instance;
            var result = helper.TryGetExport(out instance);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(StubCompositionTestImplementation).FullName, instance.GetType().FullName);
        }

        [TestMethod]
        [ExpectedException(typeof(CompositionFailedException))]
        public void GetExport_MultipleImpl_Throws()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly }, typeof(NamedCompositionTestImplementation));
            ICompositionTest stub = new StubCompositionTestImplementation();
            helper.ComposeExport(stub);

            // Act and Assert
            helper.GetExport<ICompositionTest>();
        }

        [TestMethod]
        public void TryGetExport_MultipleImpl_Fails()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly }, typeof(NamedCompositionTestImplementation));
            ICompositionTest stub = new StubCompositionTestImplementation();
            helper.ComposeExport(stub);

            // Act
            ICompositionTest instance;
            var result = helper.TryGetExport(out instance);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(instance);
        }

        [TestMethod]
        public void GetExport_MultipleImplOneNamed_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly }, typeof(CompositionTestImplementation));
            ICompositionTest stub = new StubCompositionTestImplementation();
            helper.ComposeExport(stub);

            // Act
            var instance = helper.GetExport<ICompositionTest>();

            // Assert
            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(StubCompositionTestImplementation).FullName, instance.GetType().FullName);
        }

        [TestMethod]
        public void TryGetExport_MultipleImplOneNamed_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly }, typeof(CompositionTestImplementation));
            ICompositionTest stub = new StubCompositionTestImplementation();
            helper.ComposeExport(stub);

            // Act
            ICompositionTest instance;
            var result = helper.TryGetExport(out instance);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(StubCompositionTestImplementation).FullName, instance.GetType().FullName);
        }

        [TestMethod]
        [ExpectedException(typeof(CompositionFailedException))]
        public void GetNamedExport_MultipleImplOneNamed_Throws()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly }, typeof(CompositionTestImplementation));
            ICompositionTest stub = new StubCompositionTestImplementation();
            helper.ComposeExport(stub);

            // Act and Assert
            helper.GetExport<ICompositionTest>("NamedImplementation");
        }

        [TestMethod]
        public void TryGetNamedExport_MultipleImplOneNamed_Fails()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly }, typeof(CompositionTestImplementation));
            ICompositionTest stub = new StubCompositionTestImplementation();
            helper.ComposeExport(stub);

            // Act
            ICompositionTest instance;
            var result = helper.TryGetExport(out instance, "NamedImplementation");

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(instance);
        }

        [TestMethod]
        public void ExportableTypes_All_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly });
            PrivateObject po = new PrivateObject(helper);
            po.Invoke("CreateCompositionHost");

            // Act
            var result = helper.ExportableTypes?.ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(typeof(ICompositionTest)));
            Assert.IsTrue(result.Contains(typeof(ICompositionTest2)));
        }

        [TestMethod]
        public void ExportableTypes_TypeExcluded_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly }, typeof(CompositionTest2Implementation));
            PrivateObject po = new PrivateObject(helper);
            po.Invoke("CreateCompositionHost");

            // Act
            var result = helper.ExportableTypes?.ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.Contains(typeof(ICompositionTest)));
        }
    }

    public interface ICompositionTest
    {
    }

    public interface ICompositionTest2
    {
    }

    [Export(typeof(ICompositionTest))]
    public class CompositionTestImplementation : ICompositionTest
    {
    }

    [Export("NamedImplementation", typeof(ICompositionTest))]
    public class NamedCompositionTestImplementation : ICompositionTest
    {
    }

    internal class StubCompositionTestImplementation : ICompositionTest
    {
    }

    [Export(typeof(ICompositionTest2))]
    public class CompositionTest2Implementation : ICompositionTest2
    {
    }
}
