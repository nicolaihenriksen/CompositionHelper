using System;
using System.Linq;
using System.Collections.Generic;
using System.Composition.Hosting.Core;

namespace Nicolai.Utils.Composition.CompositionHelper
{
    /// <summary>
    /// Export descriptor used to allow us to add our own instances (i.e. stubs) into the composition.
    /// </summary>
    internal class InstanceExportDescriptorProvider : ExportDescriptorProvider
    {
        private readonly object instance;

        public InstanceExportDescriptorProvider(object instance)
        {
            this.instance = instance;
        }

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            var temp = new Type[0]; // Figure out whether this.instance is an instance of type contract.ContractType
            if (temp.Where(t => Equals(t, contract.ContractType)).Any())
            {
                yield return new ExportDescriptorPromise(contract, contract.ContractType.FullName, true, NoDependencies, dependencies => ExportDescriptor.Create((context, operation) => this.instance, NoMetadata));
            }
        }

    }
}
