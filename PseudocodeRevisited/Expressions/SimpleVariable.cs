using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited.Expressions {
    /// <summary>
    /// A <see cref="IFullValue"/> that accesses a single variable.
    /// </summary>
    public sealed class SimpleVariable : IFullValue {
        /// <summary>
        /// The name of the variable.
        /// </summary>
        public string Identifier { get; private set; }
        public SimpleVariable(string identifier) {
            Identifier = identifier;
        }
        public void SetValue(ExecutionState e, object v) {
            e.Vars.SetVariable(Identifier, v);
        }
        public object GetValue(ExecutionState e) {
            return e.Vars.GetVariable(Identifier);
        }
    }
}
