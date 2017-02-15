using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited.Statements {
    /// <summary>
    /// Exits from a function, returning the result of an expression.
    /// </summary>
    public sealed class Return : Statement {
        public Expression RetVal { get; private set; }
        public Return(int lineNumber, Expression retval) : base(lineNumber) {
            RetVal = retval;
        }
        protected override Statement RunGetNextInternal(ExecutionState s) {
            s.ReturnValue = RetVal.GetValue(s);
            return null;
        }
    }
    /// <summary>
    /// Exits from a function, returning null.
    /// </summary>
    public sealed class ReturnNothing : Statement {
        public ReturnNothing(int lineNumber) : base(lineNumber) {

        }
        protected override Statement RunGetNextInternal(ExecutionState s) {
            s.ReturnValue = null;
            return null;
        }
    }
}
