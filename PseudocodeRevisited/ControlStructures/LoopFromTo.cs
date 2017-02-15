using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PseudocodeRevisited.FromToInternals;

namespace PseudocodeRevisited.ControlStructures {
    /// <summary>
    /// Represents a loop of the form "loop VAR from LOWER to UPPER".
    /// </summary>
    public sealed class LoopFromTo : ControlStructure {
        /// <summary>
        /// The <see cref="StartLoop"/> that initializes the loop.
        /// </summary>
        private StartLoop StartLoop;
        /// <summary>
        /// The <see cref="UpperBoundConditional"/> that detects when the loop variable has reached its upper bound.
        /// </summary>
        private UpperBoundConditional Conditional;
        /// <summary>
        /// The identifier for the loop variable.
        /// </summary>
        public string LoopVar { get { return StartLoop.LoopVar; } }
        /// <summary>
        /// An <see cref="Expression"/> to calculate to get the lower bound of the loop.
        /// </summary>
        public Expression Lower { get { return StartLoop.LowerBound; } }
        /// <summary>
        /// An <see cref="Expression"/> to calculate to get the upper bound of the loop.
        /// </summary>
        public Expression Upper { get { return StartLoop.UpperBound; } }
        /// <summary>
        /// The last statement added to the loop body.
        /// </summary>
        private Statement LastStatement;
        /// <summary>
        /// Creates a new from-to loop.
        /// </summary>
        public LoopFromTo(string loopvar, Expression lower, Expression upper) {
            StartLoop = new StartLoop(loopvar, lower, upper);
            FirstStatement = StartLoop;
            Conditional = new UpperBoundConditional(upper.LineNumber, loopvar);
            StartLoop.NormalNext = Conditional;
            LastStatement = Conditional.FalseBranch;
        }
        public override string Name { get { return "loop"; } }
        /// <summary>
        /// Adds statements to the loop body.
        /// </summary>
        public override void AddStatements(Statement first, Statement last) {
            LastStatement.NormalNext = first;
            LastStatement = last;
        }
        /// <summary>
        /// Finalizes and closes the loop.
        /// </summary>
        protected override Statement FinishProtected(int lineNumber) {
            StepLoop step = new StepLoop(lineNumber, LoopVar);
            StartLoop.ContinueLocation = step;
            LastStatement.NormalNext = step;
            step.NormalNext = Conditional;
            Statements.PopContext final = new Statements.PopContext(lineNumber);
            StartLoop.BreakLocation = final;
            Conditional.TrueBranch.NormalNext = final;
            return final;
        }
    }
}
