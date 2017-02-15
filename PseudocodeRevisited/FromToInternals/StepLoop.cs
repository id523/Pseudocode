using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited.FromToInternals {
    /// <summary>
    /// A <see cref="Statement"/> that represents incrementing the loop variable of a from-to loop.
    /// </summary>
    internal sealed class StepLoop : Statement {
        /// <summary>
        /// The identifier for the loop variable.
        /// </summary>
        public string Identifier { get; private set; }
        public StepLoop(int lineNumber, string id) : base(lineNumber) {
            Identifier = id;
        }
        /// <summary>
        /// Increments the loop variable.
        /// </summary>
        /// <param name="s"></param>
        protected override void Run(ExecutionState s) {
            object nextValue = Arithmetic.Increment(s.Vars.GetVariable(Identifier));
            s.Vars.SetVariable(Identifier, nextValue);
        }
    }
}
