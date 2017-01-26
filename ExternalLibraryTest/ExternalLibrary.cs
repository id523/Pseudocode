using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PseudocodeRevisited;

namespace ExternalLibraryTest
{
    public class ExternalLibrary : IExternalLibrary
    {
        public void Populate(Library lib)
        {
            lib.AddFunction("externalTest.sayHello", "sayHello", SayHello);
        }
        public static object SayHello(ExecutionState s, object[] args)
        {
            ConsoleColor oldFGColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Hello!");
            Console.ForegroundColor = oldFGColor;
            return null;
        }
    }
}
