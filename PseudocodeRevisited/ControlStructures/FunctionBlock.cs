using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited.ControlStructures {
    /// <summary>
    /// Represents a function definition, and contains the body of a function.
    /// </summary>
    public sealed class FunctionBlock : ControlStructure {
        /// <summary>
        /// The string "function".
        /// </summary>
        public override string Name { get { return "function"; } }
        /// <summary>
        /// The <see cref="Statements.DefineFunction"/> for this function.
        /// </summary>
        private Statements.DefineFunction def;
        /// <summary>
        /// The last statement added to the function.
        /// </summary>
        private Statement LastStatement;
        /// <summary>
        /// Creates a new, empty function definition with the specified name and comma-separated parameter list.
        /// </summary>
        public FunctionBlock(int lineNumber, string funcname, string args) {
            def = new Statements.DefineFunction(lineNumber, funcname, args);
            FirstStatement = def;
            LastStatement = new Statement(lineNumber);
            def.FunctionStart = LastStatement;
        }
        /// <summary>
        /// Adds consecutive statements to the body of the function.
        /// </summary>
        public override void AddStatements(Statement first, Statement last) {
            LastStatement.NormalNext = first;
            LastStatement = last;
        }

        protected override Statement FinishProtected(int lineNumber) {
            return def;
        }
    }
}
