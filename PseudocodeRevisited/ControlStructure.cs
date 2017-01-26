using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited
{
    public abstract class ControlStructure
    {
        /// <summary>
        /// Add a block of contiguous statements, where the first statement is 'first'
        /// and the last statement is 'last'.
        /// </summary>
        public abstract void AddStatements(Statement first, Statement last);
        /// <summary>
        /// Add a single statement.
        /// </summary>
        public void AddStatement(Statement s)
        {
            AddStatements(s, s);
        }
        /// <summary>
        /// Gets the first statement of the control structure.
        /// This must be set in the constructor of any derived types
        /// so that it can be added to an outer control structure.
        /// </summary>
        public Statement FirstStatement { get; protected set; }
        /// <summary>
        /// Adds the last section of the control structure
        /// (for example, a connection back to the beginning of the loop).
        /// This is called by the Finish() function.
        /// </summary>
        protected abstract Statement FinishProtected(int lineNumber);
        private bool Valid = true;
        /// <summary>
        /// Calls FinishProtected() if it has not already been called.
        /// </summary>
        public Statement Finish(int lineNumber)
        {
            if (Valid)
            {
                return FinishProtected(lineNumber);
            }
            else
            {
                throw new CompileException("Control structure " + Name + " has already been finished");
            }
        }
        /// <summary>
        /// Returns a name for the control structure. The line to end the control structure must read "end [Name]".
        /// </summary>
        public abstract string Name { get; }
    }
}
