using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited.ControlStructures {
    /// <summary>
    /// Represents a collection of statements surrounded by
    /// PushContext and PopContext instructions,
    /// and which can be broken out of with 'break'.
    /// </summary>
    public sealed class IsolatedBlock : ControlStructure {
        // The last statement added to the Block.
        private Statement LastStatement;
        // The initial PushContext instruction. Required to set the location to 'break' to later.
        private Statements.PushContext PushContext;
        /// <summary>
        /// Creates a new, empty <see cref="IsolatedBlock"/>.
        /// </summary>
        public IsolatedBlock(int lineNumber) {
            PushContext = new Statements.PushContext(lineNumber);
            FirstStatement = PushContext;
            LastStatement = FirstStatement;
        }
        /// <summary>
        /// Appends consecutive <see cref="Statement"/>s to the <see cref="IsolatedBlock"/>.
        /// </summary>
        public override void AddStatements(Statement first, Statement last) {
            LastStatement.NormalNext = first;
            LastStatement = last;
        }
        /// <summary>
        /// The string "block".
        /// </summary>
        public override string Name { get { return "block"; } }
        /// <summary>
        /// Adds the PopContext instruction to the <see cref="IsolatedBlock"/>
        /// and allows the block to be broken out of.
        /// </summary>
        protected override Statement FinishProtected(int lineNumber) {
            LastStatement.NormalNext = new Statements.PopContext(lineNumber);
            LastStatement = LastStatement.NormalNext;
            PushContext.BreakLocation = LastStatement;
            return LastStatement;
        }
    }
}
