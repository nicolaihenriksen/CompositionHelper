using System;
using System.Linq;
using System.Collections.Generic;
using System.Composition.Hosting.Core;

namespace Nicolai.Utils.Composition.CompositionHelper
{
    /// <summary>
    /// Export descriptor used to allow us to add our own instances (i.e. stubs) into the composition.
    /// </summary>
    internal class InstanceExportDescriptorProvider<T> : ExportDescriptorProvider
    {
        private readonly T instance;

        public InstanceExportDescriptorProvider(T instance)
        {
            this.instance = instance;
        }

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            if (typeof(T) == contract.ContractType)
            {
                yield return new ExportDescriptorPromise(contract, contract.ContractType.FullName, true, NoDependencies, dependencies => ExportDescriptor.Create((context, operation) => this.instance, NoMetadata));
            }
        }

    }
}
