# MEF2 Composition Helper
Microsoft.Composition helper utility which encapsulates the composition and provides easy access to some otherwise not so apparent functionality.

The main purpose of this utility is to allow MEF2 composition to be used in Unit Tests in conjunction with a mock/stub framework. With the default MEF2 composition, it is not really possible (to my knowledge) to override a type which is already included in the composition. A very concrete example of this is in the scenario where you have 2 types located in a library (DLL) which both are decorated with Export attributes, and you want to test one of the types and use a stub of the second type to do that. With MEF1 (i.e. System.ComponentModel.Composition) this was easily achieveable by simply adding the library and using /ComposeExport(myStub)/ to override the original with a stub.

For example, if you have 2 libraries: 1) one containing 2 interfaces (interfacesAssembly) and 2) one containing 2 classes (implementationAssembly) implementing those interfaces and both decorated with MEF2 Export attributes. Imagine you want to create a Unit Test where the second implementation is replaced with a stub. With this utility, you can use MEF1-like syntax to do the following:

```cs
// Create stub of Interface2
var stubInterface2 = Substitute.For<Interface2>();
// ... Setup stub properties/methods if required ...

// Create
var helper = new CompositionHelper()
	.AddAssembly(this.implementationAssembly, typeof(Implementation2));
helper.ComposeExport(stubInterface2);
```

The interesting lines here are:
```cs
var helper = new CompositionHelper()
	.AddAssembly(this.implementationAssembly, typeof(Implementation2));
```
which creates the composition helper and adds the implementation assembly while ensuring that *Implementation2* is not added to the composition, and:
```cs
helper.ComposeExport(stubInterface2);
```
which puts the stub implementation of *Interface2* in the composition.

The helper also keeps track of which exports it discovers while creating the composition. The types are available via the ExportableTypes property:
```cs
var exportableTypes = helper.ExportableTypes;
```
