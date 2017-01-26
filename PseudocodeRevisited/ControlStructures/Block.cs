using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited.ControlStructures
{
    /// <summary>
    /// Represents a collection of statements.
    /// </summary>
    public sealed class Block : ControlStructure
    {
        /// <summary>
        /// The last statement added to the Block.
        /// </summary> 
        private Statement LastStatement;
        /// <summary>
        /// Creates a new, empty <see cref="Block"/>.
        /// </summary>
        public Block(int lineNumber)
        {
            FirstStatement = new Statement(lineNumber);
            LastStatement = FirstStatement;
        }
        /// <summary>
        /// Appends consecutive <see cref="Statement"/>s to the <see cref="Block"/>.
        /// </summary>
        public override void AddStatements(Statement first, Statement last)
        {
            LastStatement.NormalNext = first;
            LastStatement = last;
        }
        /// <summary>
        /// The string "block".
        /// </summary>
        public override string Name { get { return "block"; } }
        /// <summary>
        /// Finishes the <see cref="Block"/>.
        /// </summary>
        protected override Statement FinishProtected(int lineNumber)
        {
            return LastStatement;
        }
    }
}
