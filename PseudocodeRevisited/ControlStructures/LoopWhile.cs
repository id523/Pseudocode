using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited.ControlStructures {
    /// <summary>
    /// Represents a loop of the form "loop while [condition]"
    /// </summary>
    public sealed class LoopWhile : ControlStructure {
        /// <summary>
        /// The last statement added to the loop body.
        /// </summary>
        private Statement LastStatement;
        /// <summary>
        /// The initial <see cref="Statements.PushContext"/> of the loop.
        /// </summary>
        private Statements.PushContext PushContext;
        /// <summary>
        /// The conditional statement of the loop.
        /// </summary>
        private Statements.Conditional Conditional;
        public LoopWhile(Expression cond) {
            PushContext = new Statements.PushContext(cond.LineNumber);
            FirstStatement = PushContext;
            Conditional = new Statements.Conditional(cond);
            PushContext.NormalNext = Conditional;
            LastStatement = Conditional.TrueBranch;
            PushContext.ContinueLocation = Conditional;
        }
        /// <summary>
        /// Adds consecutive statements to the loop.
        /// </summary>
        public override void AddStatements(Statement first, Statement last) {
            LastStatement.NormalNext = first;
            LastStatement = last;
        }
        /// <summary>
        /// Finalizes and closes the loop.
        /// </summary>
        protected override Statement FinishProtected(int lineNumber) {
            LastStatement.NormalNext = Conditional;
            Statements.PopContext final = new Statements.PopContext(lineNumber);
            Conditional.FalseBranch.NormalNext = final;
            PushContext.BreakLocation = final;
            return final;
        }
        /// <summary>
        /// The string "loop".
        /// </summary>
        public override string Name { get { return "loop"; } }
    }
}
