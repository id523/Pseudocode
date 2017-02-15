using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PseudocodeRevisited.Expressions;

namespace PseudocodeRevisited.FromToInternals {
    /// <summary>
    /// A conditional branch statement that chooses an execution path based on whether the loop variable
    /// is greater than the upper bound. This is used in a from-to loop.
    /// </summary>
    internal sealed class UpperBoundConditional : Statement {
        /// <summary>
        /// The statement which is jumped to if the condition is true.
        /// </summary>
        public Statement TrueBranch { get; private set; }
        /// <summary>
        /// The statement which is jumped to if the condition is false.
        /// </summary>
        public Statement FalseBranch { get { return this; } }
        /// <summary>
        /// The identifier for the loop variable.
        /// </summary>
        public string Identifier { get; private set; }
        public UpperBoundConditional(int lineNumber, string id) : base(lineNumber) {
            Identifier = id;
            TrueBranch = new Statement(lineNumber);
        }
        protected override Statement RunGetNextInternal(ExecutionState s) {
            bool Condition = Arithmetic.Compare(s.Vars.GetVariable(Identifier), s.Vars.LoopUpperBound) > 0;
            if (Condition)
                return TrueBranch.NormalNext;
            else
                return NormalNext;
        }
    }
}
