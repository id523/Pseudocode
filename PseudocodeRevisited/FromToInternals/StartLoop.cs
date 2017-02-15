using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited.FromToInternals {
    /// <summary>
    /// Initializes a from-to loop.
    /// </summary>
    internal sealed class StartLoop : Statement {
        /// <summary>
        /// The identifier for the loop variable.
        /// </summary>
        public string LoopVar { get; private set; }
        /// <summary>
        /// An <see cref="Expression"/> to calculate to get the lower bound of the loop.
        /// </summary>
        public Expression LowerBound { get; private set; }
        /// <summary>
        /// An <see cref="Expression"/> to calculate to get the upper bound of the loop.
        /// </summary>
        public Expression UpperBound { get; private set; }
        /// <summary>
        /// The statement to 'break' to.
        /// </summary>
        public Statement BreakLocation { get; set; }
        /// <summary>
        /// The statement to 'continue' to.
        /// </summary>
        public Statement ContinueLocation { get; set; }
        public StartLoop(string loopvar, Expression lower, Expression upper) : base(lower.LineNumber) {
            LoopVar = loopvar;
            LowerBound = lower;
            UpperBound = upper;
        }
        protected override void Run(ExecutionState s) {
            s.PushContext();
            if (BreakLocation != null)
                s.Vars.BreakLocation = BreakLocation;
            if (ContinueLocation != null)
                s.Vars.ContinueLocation = ContinueLocation;
            s.Vars.SetVariable(LoopVar, LowerBound.GetValue(s));
            s.Vars.LoopUpperBound = UpperBound.GetValue(s);
        }
    }
}
