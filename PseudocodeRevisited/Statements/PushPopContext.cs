using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited.Statements
{
    /// <summary>
    /// A Statement that enters a new local <see cref="Scope"/>.
    /// </summary>
    public class PushContext : Statement
    {
        public Statement BreakLocation { get; set; }
        public Statement ContinueLocation { get; set; }
        public PushContext(int lineNumber) : base(lineNumber)
        {
            BreakLocation = null;
            ContinueLocation = null;
        }
        protected override void Run(ExecutionState s)
        {
            s.PushContext();
            if (BreakLocation != null)
                s.Vars.BreakLocation = BreakLocation;
            if (ContinueLocation != null)
                s.Vars.ContinueLocation = ContinueLocation;
        }
    }
    /// <summary>
    /// A Statement that exits out of the current <see cref="Scope"/>.
    /// </summary>
    public class PopContext : Statement
    {
        public PopContext(int lineNumber) : base(lineNumber) { }
        protected override void Run(ExecutionState s)
        {
            s.PopContext();
        }
    }
}
