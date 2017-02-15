using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PseudocodeRevisited {
    /// <summary>
    /// This exception is produced when there is an error while running a pseudocode program.
    /// </summary>
    [Serializable]
    public class RuntimeException : Exception {
        public RuntimeException() : base() { }
        public RuntimeException(string message) : base(message) { }
        public RuntimeException(string message, Exception innerException) : base(message, innerException) { }
        public RuntimeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
    /// <summary>
    /// This exception is produced when there is an error while parsing/compiling a pseudocode program.
    /// </summary>
    [Serializable]
    public class CompileException : Exception {
        public CompileException() : base() { }
        public CompileException(string message) : base(message) { }
        public CompileException(string message, Exception innerException) : base(message, innerException) { }
        public CompileException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
