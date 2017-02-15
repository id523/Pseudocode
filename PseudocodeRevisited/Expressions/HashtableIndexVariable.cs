using System;
using System.Collections;

namespace PseudocodeRevisited.Expressions {
    /// <summary>
    /// A <see cref="IFullValue"/> that accesses the item with a specified key in an <see cref="IDictionary"/>.
    /// </summary>
    public sealed class HashtableIndexVariable : IFullValue {
        public IDictionary TableAccessed { get; private set; }
        public object Key { get; private set; }
        public HashtableIndexVariable(IDictionary tbl, object key) {
            TableAccessed = tbl;
            Key = key;
        }

        public object GetValue(ExecutionState s) {
            try {
                if (!TableAccessed.Contains(Key))
                    throw new RuntimeException("The key is not present in the hashtable");
                return TableAccessed[Key];
            } catch (RuntimeException ex) {
                throw ex;
            } catch (Exception) {
                throw new RuntimeException("Invalid key for a hashtable");
            }
        }

        public void SetValue(ExecutionState s, object v) {
            try {
                TableAccessed[Key] = v;
            } catch (Exception) {
                throw new RuntimeException("The value could not be stored in the hashtable");
            }
        }
    }
}
