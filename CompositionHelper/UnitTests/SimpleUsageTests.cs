using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nicolai.Utils.Composition.CompositionHelper;
using System.Composition.Hosting;
using System.Composition;
using System.Linq;
using System.Composition.Convention;
using System;

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
        public void Dispose_StubDisposed_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly }, typeof(CompositionTestImplementation));
            ICompositionTest stub = new StubCompositionTestImplementation();
            helper.ComposeExport(stub);
            var instance = helper.GetExport<ICompositionTest>();

            // Act
            helper.Dispose(true);

            // Assert
            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(StubCompositionTestImplementation).FullName, instance.GetType().FullName);
            Assert.IsTrue(instance.DisposeCalled);
        }

        [TestMethod]
        public void Dispose_StubNotDisposed_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly }, typeof(CompositionTestImplementation));
            ICompositionTest stub = new StubCompositionTestImplementation();
            helper.ComposeExport(stub);
            var instance = helper.GetExport<ICompositionTest>();

            // Act
            helper.Dispose(false);

            // Assert
            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(StubCompositionTestImplementation).FullName, instance.GetType().FullName);
            Assert.IsFalse(instance.DisposeCalled);
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
        public void GetExports_Generic_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly });

            // Act
            var types = helper.GetExports<ICompositionTest3>()?.ToList();

            // Assert
            Assert.IsNotNull(types);
            Assert.AreEqual(2, types.Count);
            Assert.IsTrue(types.Where(t => t.GetType() == typeof(CompositionTest3Implementation1)).Any());
            Assert.IsTrue(types.Where(t => t.GetType() == typeof(CompositionTest3Implementation2)).Any());
        }

        [TestMethod]
        public void GetExports_NamedGeneric_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly });

            // Act
            var types = helper.GetExports<ICompositionTest3>("NamedImplementation")?.ToList();

            // Assert
            Assert.IsNotNull(types);
            Assert.AreEqual(2, types.Count);
            Assert.IsTrue(types.Where(t => t.GetType() == typeof(NamedCompositionTest3Implementation1)).Any());
            Assert.IsTrue(types.Where(t => t.GetType() == typeof(NamedCompositionTest3Implementation2)).Any());
        }

        [TestMethod]
        public void GetExports_NonGeneric_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly });

            // Act
            var types = helper.GetExports(typeof(ICompositionTest3))?.ToList();

            // Assert
            Assert.IsNotNull(types);
            Assert.AreEqual(2, types.Count);
            Assert.IsTrue(types.Where(t => t.GetType() == typeof(CompositionTest3Implementation1)).Any());
            Assert.IsTrue(types.Where(t => t.GetType() == typeof(CompositionTest3Implementation2)).Any());
        }

        [TestMethod]
        public void GetExports_NamedNonGeneric_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly });

            // Act
            var types = helper.GetExports(typeof(ICompositionTest3), "NamedImplementation")?.ToList();

            // Assert
            Assert.IsNotNull(types);
            Assert.AreEqual(2, types.Count);
            Assert.IsTrue(types.Where(t => t.GetType() == typeof(NamedCompositionTest3Implementation1)).Any());
            Assert.IsTrue(types.Where(t => t.GetType() == typeof(NamedCompositionTest3Implementation2)).Any());
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
            Assert.AreEqual(3, result.Count);
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
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(typeof(ICompositionTest)));
        }

        [TestMethod]
        public void GetExport_NonShared_Success()
        {
            // Arrange
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly });

            // Act
            var instance1 = helper.GetExport<ICompositionTest>();
            var instance2 = helper.GetExport<ICompositionTest>();

            // Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.AreNotEqual(instance1, instance2);
            Assert.AreEqual(typeof(CompositionTestImplementation).FullName, instance1.GetType().FullName);
            Assert.AreEqual(typeof(CompositionTestImplementation).FullName, instance2.GetType().FullName);
        }

        [TestMethod]
        public void GetExport_NonSharedOverrideToShared_Success()
        {
            // Arrange
            var conventions = new ConventionBuilder();
            conventions.ForTypesMatching(t => true).Shared();
            var helper = new CompositionHelper().AddAssemblies(new[] { GetType().Assembly }, conventions);

            // Act
            var instance1 = helper.GetExport<ICompositionTest>();
            var instance2 = helper.GetExport<ICompositionTest>();

            // Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.AreEqual(instance1, instance2);
            Assert.AreEqual(typeof(CompositionTestImplementation).FullName, instance1.GetType().FullName);
        }

        [TestMethod]
        public void GetExport_Shared_Success()
        {
            // Arrange
            var helper = new CompositionHelper(true).AddAssemblies(new[] { GetType().Assembly });

            // Act
            var instance1 = helper.GetExport<ICompositionTest>();
            var instance2 = helper.GetExport<ICompositionTest>();

            // Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.AreEqual(instance1, instance2);
            Assert.AreEqual(typeof(CompositionTestImplementation).FullName, instance1.GetType().FullName);
        }

        [TestMethod]
        public void GetExport_SharedOverrideToNonShared_Success()
        {
            // Arrange
            var helper = new CompositionHelper(true).AddAssemblies(new[] { GetType().Assembly }, new ConventionBuilder());

            // Act
            var instance1 = helper.GetExport<ICompositionTest>();
            var instance2 = helper.GetExport<ICompositionTest>();

            // Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.AreNotEqual(instance1, instance2);
            Assert.AreEqual(typeof(CompositionTestImplementation).FullName, instance1.GetType().FullName);
            Assert.AreEqual(typeof(CompositionTestImplementation).FullName, instance2.GetType().FullName);
        }
    }

    public interface ICompositionTest
    {
        bool DisposeCalled { get; }
    }

    public interface ICompositionTest2
    { }

    public interface ICompositionTest3
    { }

    [Export(typeof(ICompositionTest))]
    public class CompositionTestImplementation : ICompositionTest
    {
        public bool DisposeCalled => false;
    }

    [Export("NamedImplementation", typeof(ICompositionTest))]
    public class NamedCompositionTestImplementation : ICompositionTest
    {
        public bool DisposeCalled => false;
    }

    internal class StubCompositionTestImplementation : ICompositionTest, IDisposable
    {
        public bool DisposeCalled { get; private set; }
        public void Dispose()
        {
            DisposeCalled = true;
        }
    }

    [Export(typeof(ICompositionTest2))]
    public class CompositionTest2Implementation : ICompositionTest2
    { }

    [Export(typeof(ICompositionTest3))]
    public class CompositionTest3Implementation1 : ICompositionTest3
    { }

    [Export(typeof(ICompositionTest3))]
    public class CompositionTest3Implementation2 : ICompositionTest3
    { }

    [Export("NamedImplementation", typeof(ICompositionTest3))]
    public class NamedCompositionTest3Implementation1 : ICompositionTest3
    { }

    [Export("NamedImplementation", typeof(ICompositionTest3))]
    public class NamedCompositionTest3Implementation2 : ICompositionTest3
    { }
}
