using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited {
    /// <summary>
    /// Represents an action which can be performed by a pseudocode program.
    /// </summary>
    public class Statement {
        /// <summary>
        /// The line number that this Statement corresponds to.
        /// </summary>
        public int LineNumber { get; private set; }
        public Statement(int lineNumber) {
            LineNumber = lineNumber;
        }
        public Statement NormalNext { get; set; }
        /// <summary>
        /// Does nothing by default but can be overridden in a derived class to perform an action.
        /// </summary>
        protected virtual void Run(ExecutionState s) { }
        /// <summary>
        /// Performs this statement's action and returns the next statement to execute (for conditional
        /// branching or other special operations).
        /// </summary>
        protected virtual Statement RunGetNextInternal(ExecutionState s) {
            Run(s);
            return NormalNext;
        }
        public Statement RunGetNext(ExecutionState s) {
            try {
                return RunGetNextInternal(s);
            } catch (RuntimeException ex) {
                throw new RuntimeException(LineNumber.ToString() + ": " + ex.Message);
            }
        }
    }
}
