using System.Composition;
using TestInterfaces;

namespace TestImplementations
{
    [Export(typeof(Interface2))]
    public class Implementation2 : Interface2
    {
        public string GetMessage()
        {
            return "Hello from " + this.GetType().Name;
        }
    }
}
