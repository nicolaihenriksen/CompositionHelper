using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Reflection;

namespace Nicolai.Utils.Composition.CompositionHelper
{
    /// <summary>
    /// Helper class which is useful for Unit Testing when using MEF2 (i.e. Microsoft.Composition).
    /// </summary>
    public class CompositionHelper
    {
        private ContainerConfiguration config = new ContainerConfiguration();

        private CompositionHost host;

        public CompositionHelper AddAssembly(Assembly assembly, params Type[] excludedTypes)
        {
            return AddAssemblies(new[] { assembly }, excludedTypes);
        }

        public CompositionHelper AddAssemblies(IEnumerable<Assembly> assemblies, params Type[] excludedTypes)
        {
            this.config = this.config.WithAssemblies(assemblies, new CustomConventionBuilder(excludedTypes));
            return this;
        }

        public CompositionHelper AddType<TPart>()
        {
            this.config = this.config.WithPart<TPart>();
            return this;
        }

        public CompositionHelper AddType(Type type)
        {
            this.config = this.config.WithPart(type);
            return this;
        }

        public CompositionHelper ComposeExport<TPart>(TPart implementation)
        {
            if (implementation != null)
                this.config = this.config.WithProvider(new InstanceExportDescriptorProvider<TPart>(implementation));
            return this;
        }

        public object GetExport(Type type, string contractName = null)
        {
            CreateCompositionHost();
            return string.IsNullOrEmpty(contractName) ? this.host.GetExport(type) : this.host.GetExport(type, contractName);
        }

        public T GetExport<T>(string contractName = null)
        {
            CreateCompositionHost();
            return string.IsNullOrEmpty(contractName) ? this.host.GetExport<T>() : this.host.GetExport<T>(contractName);
        }

        public IEnumerable<object> GetExports(Type type, string contractName = null)
        {
            CreateCompositionHost();
            return string.IsNullOrEmpty(contractName) ? this.host.GetExports(type) : this.host.GetExports(type, contractName);
        }

        public IEnumerable<T> GetExports<T>(string contractName = null)
        {
            CreateCompositionHost();
            return string.IsNullOrEmpty(contractName) ? this.host.GetExports<T>() : this.host.GetExports<T>(contractName);
        }

        public bool TryGetExport(Type type, out object export, string contractName = null)
        {
            CreateCompositionHost();
            return string.IsNullOrEmpty(contractName) ? this.host.TryGetExport(type, out export) : this.host.TryGetExport(type, contractName, out export);
        }

        public bool TryGetExport<T>(out T export, string contractName = null)
        {
            CreateCompositionHost();
            return string.IsNullOrEmpty(contractName) ? this.host.TryGetExport<T>(out export) : this.host.TryGetExport<T>(contractName, out export);
        }

        private void CreateCompositionHost()
        {
            lock (this.config)
            {
                if (this.host == null)
                {
                    this.host = this.config.CreateContainer();
                }
            }
        }
    }
}
