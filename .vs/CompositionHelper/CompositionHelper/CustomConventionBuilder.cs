using System;
using System.Collections.Generic;
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

        private readonly List<Type> ignoredTypes = new List<Type>();

        public CustomConventionBuilder(params Type[] ignoredTypes)
        {
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
            return base.GetCustomAttributes(reflectedType, member);
        }

    }
}
