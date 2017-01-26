using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited
{
    /// <summary>
    /// This exception is produced when there is an error while running a pseudocode program.
    /// </summary>
    [Serializable]public class RuntimeException : Exception
    {
        public RuntimeException(string message) : base(message)
        {
        }
    }
    /// <summary>
    /// This exception is produced when there is an error while parsing/compiling a pseudocode program.
    /// </summary>
    [Serializable]public class CompileException : Exception
    {
        public CompileException(string message) : base(message)
        {
        }
    }
}
