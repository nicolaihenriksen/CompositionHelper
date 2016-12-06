# MEF2 Composition Helper
Microsoft.Composition helper utility which encapsulates the composition and provides easy access to some otherwise not so apparent functionality.

The main purpose of this utility is to allow MEF2 composition to be used in Unit Tests in conjunction with a mock/stub framework. With the default MEF2 composition, it is not really possible (to my knowledge) to override a type which is already included in the composition. A very concrete example of this is in the scenario where you have 2 types located in a library (DLL) which both are decorated with Export attributes, and you want to test one of the types and use a stub of the second type to do that. With MEF1 (i.e. System.ComponentModel.Composition) this was easily achieveable by simply adding the library and using /ComposeExport(myStub)/ to override the original with a stub.

´´´C#
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
´´´ 
