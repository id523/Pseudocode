using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited.Statements
{
    /// <summary>
    /// Exits the enclosing loop, if that is possible.
    /// </summary>
    public sealed class Break : Statement
    {
        public Break(int lineNumber) : base(lineNumber) { }
        protected override Statement RunGetNextInternal(ExecutionState s)
        {
            return s.Break();
        }
    }
    /// <summary>
    /// Starts the next iteration the enclosing loop, if that is possible.
    /// </summary>
    public sealed class Continue : Statement
    {
        public Continue(int lineNumber) : base(lineNumber) { }
        protected override Statement RunGetNextInternal(ExecutionState s)
        {
            return s.Continue();
        }
    }
}
