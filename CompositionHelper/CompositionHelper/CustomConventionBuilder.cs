using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Reflection;

namespace Nicolai.Utils.Composition.CompositionHelper
{
    /// <summary>
    /// Custom convention builder used to filter out certain exported types from an assembly.
    /// 
    /// This is used when adding an assembly to the composition, but wanting to compose some implementations in directly.
    /// In standard MEF this could be done with ComposeExport(), but since that is not included in Microsoft.Composition
    /// we have to filter out the types we want to override up front.
    /// </summary>
    internal class CustomConventionBuilder : ConventionBuilder
    {
        private HashSet<Type> visitedTypes = new HashSet<Type>();
        private HashSet<Type> exportableTypes = new HashSet<Type>();
        public IEnumerable<Type> ExportableTypes => exportableTypes;

        private readonly AttributedModelProvider convention;

        private readonly List<Type> ignoredTypes = new List<Type>();

        public CustomConventionBuilder(AttributedModelProvider convention, params Type[] ignoredTypes)
        {
            if (convention != null)
                this.convention = convention;
            if (ignoredTypes != null)
            {
                this.ignoredTypes.AddRange(ignoredTypes);
            }
        }

        public override IEnumerable<Attribute> GetCustomAttributes(Type reflectedType, MemberInfo member)
        {
            if (this.ignoredTypes.Contains(reflectedType))
            {
                return new Attribute[0];
            }
            IEnumerable<Attribute> result;
            if (convention != null)
                result = convention.GetCustomAttributes(reflectedType, member);
            else
                result = base.GetCustomAttributes(reflectedType, member);
            AddExportedType(reflectedType, result);
            return result;
        }

        public override IEnumerable<Attribute> GetCustomAttributes(Type reflectedType, ParameterInfo parameter)
        {
            if (this.ignoredTypes.Contains(reflectedType))
            {
                return new Attribute[0];
            }
            IEnumerable<Attribute> result;
            if (convention != null)
                result = convention.GetCustomAttributes(reflectedType, parameter);
            else
                result = base.GetCustomAttributes(reflectedType, parameter);
            AddExportedType(reflectedType, result);
            return result;
        }

        private void AddExportedType(Type reflectedType, IEnumerable<Attribute> attributes)
        {
            if (this.visitedTypes.Contains(reflectedType))
                return;

            foreach (var attribute in attributes)
            {
                var exportAttrib = attribute as ExportAttribute;
                if (exportAttrib != null)
                {
                    exportableTypes.Add(exportAttrib.ContractType);
                }
            }

            this.visitedTypes.Add(reflectedType);
        }
    }
}
