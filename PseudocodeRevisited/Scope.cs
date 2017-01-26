using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited
{
    /// <summary>
    /// Stores local variables and the Statements to go to with 'break' and 'continue'.
    /// </summary>
    public sealed class Scope
    {
        /// <summary>
        /// The <see cref="Scope"/> that contains this one.
        /// </summary>
        public Scope Parent { get; private set; }
        /// <summary>
        /// Creates a new, global <see cref="Scope"/>.
        /// </summary>
        public Scope() : this(null) { }
        /// <summary>
        /// Creates a new local <see cref="Scope"/> enclosed inside the specified one.
        /// </summary>
        public Scope(Scope parent)
        {
            Parent = parent;
            Variables = new Dictionary<string, object>(32);
        }
        /// <summary>
        /// Stores the local variables in this Scope.
        /// </summary>
        private Dictionary<string, object> Variables;
        /// <summary>
        /// The location to go to with <see cref="Statements.Break"/>.
        /// </summary>
        public Statement BreakLocation { get; set; }
        /// <summary>
        /// The location to go to with a <see cref="Statements.Continue"/>.
        /// </summary>
        public Statement ContinueLocation { get; set; }
        /// <summary>
        /// Gets or sets the upper bound for the from-to loop in this Scope.
        /// If there is no such loop, this value is ignored.
        /// </summary>
        public object LoopUpperBound { get; set; }
        /// <summary>
        /// Sets a variable in this scope or any enclosing scope.
        /// </summary>
        public void SetVariable(string identifier, object value)
        {
            Scope scope = Locate(identifier) ?? this;
            scope.Variables[identifier] = value;
        }
        /// <summary>
        /// Finds the scope which contains the variable of the specified name.
        /// </summary>
        private Scope Locate(string identifier)
        {
            Scope curr = this;
            while (curr != null && !curr.Variables.ContainsKey(identifier))
            {
                curr = curr.Parent;
            }
            return curr;
        }
        /// <summary>
        /// Gets a variable in this scope or any enclosing scope.
        /// </summary>
        public object GetVariable(string identifier)
        {
            Scope scope = Locate(identifier);
            if (scope == null)
            {
                throw new RuntimeException(string.Format("Identifier {0} is not defined", identifier));
            }
            return scope.Variables[identifier];
        }
    }
}
