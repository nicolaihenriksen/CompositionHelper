using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Reflection;

namespace Nicolai.Utils.Composition.CompositionHelper
{
    /// <summary>
    /// Helper class which is useful for Unit Testing when using MEF2 (i.e. Microsoft.Composition).
    /// </summary>
    public class CompositionHelper : IDisposable
    {
        public IEnumerable<Type> ExportableTypes => exportableTypes;

        private bool disposed;
        private HashSet<Type> exportableTypes = new HashSet<Type>();
        private List<CustomConventionBuilder> builders = new List<CustomConventionBuilder>();
        private ContainerConfiguration config = new ContainerConfiguration();
        private CompositionHost host;
        private AttributedModelProvider defaultConventions;

        /// <summary>
        /// Creates the composition helper with the default conventions provided. These conventions can be
        /// overridden when adding types/assemblies.
        /// </summary>
        /// <param name="defaultConventions">The default conventions to use</param>
        public CompositionHelper(AttributedModelProvider defaultConventions)
        {
            Initialize(defaultConventions);
        }

        /// <summary>
        /// Creates the composition helper with an option to indicate whether all instances should (by default)
        /// be shared
        /// </summary>
        /// <param name="allInstancesSharedByDefault">Indication of whether or not the instances should be shared or not by default. This can be overridden when adding types/assemblies</param>
        public CompositionHelper(bool allInstancesSharedByDefault = false)
        {
            ConventionBuilder builder = null;
            if (allInstancesSharedByDefault)
            {
                builder = new ConventionBuilder();
                builder.ForTypesMatching(t => true).Shared();
            }
            Initialize(builder);
        }

        /// <summary>
        /// Initializes the configuration with a default convention if not null
        /// </summary>
        /// <param name="defaultConventions">Default convention to use in the composition</param>
        private void Initialize(AttributedModelProvider defaultConventions)
        {
            if (defaultConventions != null)
            {
                this.defaultConventions = defaultConventions;
            }
        }

        /// <summary>
        /// Adds the given assembly to the composition configuration. You can optionally provide an array of types which
        /// you do not want to be included in the composition (i.e. if you desire to use ComposeExport() to insert your own
        /// stub).
        /// </summary>
        /// <param name="assembly">The assembly from which all types, decorated with an <see cref="ExportAttribute"/> attribute, will be included in the configuration</param>
        /// <param name="excludedTypes">Array of types, decorated with an <see cref="ExportAttribute"/> attribute, which should not be included in the configuration</param>
        /// <returns>Reference to the helper itself; Builder Pattern</returns>
        public CompositionHelper AddAssembly(Assembly assembly, params Type[] excludedTypes)
        {
            VerifyCompositionHostNotCreated();
            return AddAssemblies(new[] { assembly }, null, excludedTypes);
        }

        /// <summary>
        /// Adds the given assembly to the composition configuration. You can optionally provide an array of types which
        /// you do not want to be included in the composition (i.e. if you desire to use ComposeExport() to insert your own
        /// stub).
        /// </summary>
        /// <param name="assembly">The assembly from which all types, decorated with an <see cref="ExportAttribute"/> attribute, will be included in the configuration</param>
        /// <param name="conventions">Conventions used when adding the given assembly</param>
        /// <param name="excludedTypes">Array of types, decorated with an <see cref="ExportAttribute"/> attribute, which should not be included in the configuration</param>
        /// <returns>Reference to the helper itself; Builder Pattern</returns>
        public CompositionHelper AddAssembly(Assembly assembly, AttributedModelProvider conventions, params Type[] excludedTypes)
        {
            VerifyCompositionHostNotCreated();
            return AddAssemblies(new[] { assembly }, conventions, excludedTypes);
        }

        /// <summary>
        /// Adds the given list of assemblies to the composition configuration. You can optionally provide an array of types which
        /// you do not want to be included in the composition (i.e. if you desire to use ComposeExport() to insert your own
        /// stub).
        /// </summary>
        /// <param name="assemblies">The list of assemblies from which all types, decorated with an <see cref="ExportAttribute"/> attribute, will be included in the configuration</param>
        /// <param name="excludedTypes">Array of types, decorated with an <see cref="ExportAttribute"/> attribute, which should not be included in the configuration</param>
        /// <returns>Reference to the helper itself; Builder Pattern</returns>
        public CompositionHelper AddAssemblies(IEnumerable<Assembly> assemblies, params Type[] excludedTypes)
        {
            VerifyCompositionHostNotCreated();
            return AddAssemblies(assemblies, null, excludedTypes);
        }

        /// <summary>
        /// Adds the given list of assemblies to the composition configuration. You can optionally provide an array of types which
        /// you do not want to be included in the composition (i.e. if you desire to use ComposeExport() to insert your own
        /// stub).
        /// </summary>
        /// <param name="assemblies">The list of assemblies from which all types, decorated with an <see cref="ExportAttribute"/> attribute, will be included in the configuration</param>
        /// <param name="conventions">Conventions used when adding the given assemblies</param>
        /// <param name="excludedTypes">Array of types, decorated with an <see cref="ExportAttribute"/> attribute, which should not be included in the configuration</param>
        /// <returns>Reference to the helper itself; Builder Pattern</returns>
        public CompositionHelper AddAssemblies(IEnumerable<Assembly> assemblies, AttributedModelProvider conventions, params Type[] excludedTypes)
        {
            VerifyCompositionHostNotCreated();
            var builder = new CustomConventionBuilder(conventions ?? defaultConventions, excludedTypes);
            builders.Add(builder);
            this.config = this.config.WithAssemblies(assemblies, builder);
            return this;
        }

        /// <summary>
        /// Adds the given type, T, to the composition configuration.
        /// </summary>
        /// <param name="conventions">Conventions used when adding the given type, T</param>
        /// <returns>Reference to the helper itself; Builder Pattern</returns>
        public CompositionHelper AddType<TPart>(AttributedModelProvider conventions = null)
        {
            VerifyCompositionHostNotCreated();
            this.config = conventions == null ? this.config.WithPart<TPart>(defaultConventions) : this.config.WithPart<TPart>(conventions);
            exportableTypes.Add(typeof(TPart));
            return this;
        }

        /// <summary>
        /// Adds the given type to the composition configuration.
        /// </summary>
        /// <param name="type">The type to add</param>
        /// <param name="conventions">Conventions used when adding the given type</param>
        /// <returns>Reference to the helper itself; Builder Pattern</returns>
        public CompositionHelper AddType(Type type, AttributedModelProvider conventions = null)
        {
            VerifyCompositionHostNotCreated();
            this.config = conventions == null ? this.config.WithPart(type, defaultConventions) : this.config.WithPart(type, conventions);
            exportableTypes.Add(type);
            return this;
        }

        /// <summary>
        /// Composes an instance of reference type TPart into the configuration.
        /// </summary>
        /// <param name="implementation">The implementation to compose into the configuration</param>
        /// <returns>Reference to the helper itself; Builder Pattern</returns>
        public CompositionHelper ComposeExport<TPart>(TPart implementation)
        {
            VerifyCompositionHostNotCreated();
            if (implementation != null)
                this.config = this.config.WithProvider(new InstanceExportDescriptorProvider<TPart>(implementation));
            exportableTypes.Add(typeof(TPart));
            return this;
        }

        /// <summary>
        /// Get the export of the given type. You can optionally provide a contractName matching one that has been used in the <see cref="ExportAttribute"/>
        /// of the desired type.
        /// </summary>
        /// <param name="type">The type of the desired export</param>
        /// <param name="contractName">Optional name of the desired export (used in the <see cref="ExportAttribute"/> on the type)</param>
        /// <returns>Instance of the desired type</returns>
        /// <exception cref="CompositionFailedException">If the desired type cannot be exported from the composition</exception>
        public object GetExport(Type type, string contractName = null)
        {
            CreateCompositionHost();
            return string.IsNullOrEmpty(contractName) ? this.host.GetExport(type) : this.host.GetExport(type, contractName);
        }

        /// <summary>
        /// Get the export of the given type, T. You can optionally provide a contractName matching one that has been used in the <see cref="ExportAttribute"/>
        /// of the desired type.
        /// </summary>
        /// <param name="contractName">Optional name of the desired export (used in the <see cref="ExportAttribute"/> on the type)</param>
        /// <returns>Instance of the desired type, T</returns>
        /// <exception cref="CompositionFailedException">If the desired type, T, cannot be exported from the composition</exception>
        public T GetExport<T>(string contractName = null)
        {
            CreateCompositionHost();
            return string.IsNullOrEmpty(contractName) ? this.host.GetExport<T>() : this.host.GetExport<T>(contractName);
        }

        /// <summary>
        /// Get the export of the given type. You can optionally provide a contractName matching one that has been used in the <see cref="ExportAttribute"/>
        /// of the desired type.
        /// </summary>
        /// <param name="type">The type of the desired export</param>
        /// <param name="export">The reference variable to hold the result</param>
        /// <param name="contractName">Optional name of the desired export (used in the <see cref="ExportAttribute"/> on the type)</param>
        /// <returns>True if the export was succesful, False otherwise</returns>
        public bool TryGetExport(Type type, out object export, string contractName = null)
        {
            CreateCompositionHost();
            try
            {
                return string.IsNullOrEmpty(contractName) ? this.host.TryGetExport(type, out export) : this.host.TryGetExport(type, contractName, out export);
            }
            catch (CompositionFailedException)
            {
                export = null;
                return false;
            }
        }

        /// <summary>
        /// Get the export of the given type, T. You can optionally provide a contractName matching one that has been used in the <see cref="ExportAttribute"/>
        /// of the desired type.
        /// </summary>
        /// <param name="export">The reference variable to hold the result</param>
        /// <param name="contractName">Optional name of the desired export (used in the <see cref="ExportAttribute"/> on the type)</param>
        /// <returns>True if the export was succesful, False otherwise</returns>
        public bool TryGetExport<T>(out T export, string contractName = null)
        {
            CreateCompositionHost();
            try
            {
                return string.IsNullOrEmpty(contractName) ? this.host.TryGetExport<T>(out export) : this.host.TryGetExport<T>(contractName, out export);
            }
            catch (CompositionFailedException)
            {
                export = default(T);
                return false;
            }
        }

        /// <summary>
        /// Gets all the exports of the given type, T. You can optionally provide a contractName matching one that has been used in the <see cref="ExportAttribute"/>
        /// of the desired types.
        /// </summary>
        /// <param name="contractName">Optional name of the desired exports (used in the <see cref="ExportAttribute"/> on the type)</param>
        /// <returns>Enumerable of all the exported instances matching the desired type T</returns>
        /// <exception cref="CompositionFailedException">If the desired type, T, cannot be exported from the composition</exception>
        public IEnumerable<T> GetExports<T>(string contractName = null)
        {
            CreateCompositionHost();
            return string.IsNullOrEmpty(contractName) ? this.host.GetExports<T>() : this.host.GetExports<T>(contractName);
        }

        /// <summary>
        /// Gets all the exports of the given type. You can optionally provide a contractName matching one that has been used in the <see cref="ExportAttribute"/>
        /// of the desired types.
        /// </summary>
        /// <param name="contractName">Optional name of the desired exports (used in the <see cref="ExportAttribute"/> on the type)</param>
        /// <returns>Enumerable of all the exported instances matching the desired type</returns>
        /// <exception cref="CompositionFailedException">If the desired type cannot be exported from the composition</exception>
        public IEnumerable<object> GetExports(Type type, string contractName = null)
        {
            CreateCompositionHost();
            return string.IsNullOrEmpty(contractName) ? this.host.GetExports(type) : this.host.GetExports(type, contractName);
        }

        /// <summary>
        /// Checks that the composition is not already created, if so it throws an exception
        /// </summary>
        private void VerifyCompositionHostNotCreated()
        {
            if (this.host != null)
                throw new NotSupportedException("Adding types or assemblies to the configuration after exporting a type is not supported!");
        }

        /// <summary>
        /// Creates the CompositionHost if not already created. Once created, modifications to the configuration will NOT have any effect.
        /// </summary>
        private void CreateCompositionHost()
        {
            lock (this.config)
            {
                if (this.host == null)
                {
                    this.host = this.config.CreateContainer();
                    foreach (var builder in builders)
                    {
                        foreach (var type in builder.ExportableTypes)
                        {
                            exportableTypes.Add(type);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Disposes the CompositionHelper and the underlying ContainerHost
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the CompositionHelper and the underlying ContainerHost if not already disposed
        /// </summary>
        /// <param name="disposing">Indication of whether or not to dispose</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || this.disposed)
                return;

            this.host?.Dispose();
            this.disposed = true;
        }
    }
}
