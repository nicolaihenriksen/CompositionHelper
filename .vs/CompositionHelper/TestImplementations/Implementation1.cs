using System.Composition;
using TestInterfaces;

namespace TestImplementations
{
    [Export(typeof(Interface1))]
    public class Implementation1 : Interface1
    {

        private Interface2 implementation2;

        [ImportingConstructor]
        public Implementation1(Interface2 implementation2)
        {
            this.implementation2 = implementation2;
        }

        public string GetInnerMessage()
        {
            return this.implementation2.GetMessage();
        }
    }
}
