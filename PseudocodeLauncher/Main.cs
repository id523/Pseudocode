using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PseudocodeRevisited;
using PseudocodeRevisited.ControlStructures;

namespace PseudocodeLauncher
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Read a program from a file or just read an example program.
            string program;
            if (args.Length < 1)
            {
#if DEBUG
                program = ExamplePrograms.TestProgram;
#else
                program = ExamplePrograms.Factors;
#endif
            }
            else
            {
                program = System.IO.File.ReadAllText(args[0]);
            }
            ExecutionState es = new ExecutionState();
            // Compile and run the code
            try
            {
                es.NextStatement = ControlStructParsing.MakeProgram(program);
                es.Run();
            }
            catch (CompileException ex)
            {
                Console.WriteLine("Compile error: " + ex.Message);
            }
            catch (RuntimeException ex)
            {
                Console.WriteLine("Runtime error: " + ex.Message);
            }
            Console.WriteLine("Program complete");
            Console.ReadLine();
        }
    }
}
