using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited.Expressions
{
    /// <summary>
    /// A <see cref="IFullValue"/> that accesses an index into an array.
    /// </summary>
    public sealed class ArrayIndexVariable : IFullValue
    {
        public object[] ArrayAccessed { get; private set; }
        public int Index { get; private set; }
        public ArrayIndexVariable(object[] arr, object idx)
        {
            ArrayAccessed = arr;
            try
            {
                Index = Convert.ToInt32(idx);
            }
            catch (Exception)
            {
                throw new RuntimeException("Indices into arrays must be whole numbers");
            }
            if (Index < 0 || Index >= ArrayAccessed.Length)
            {
                throw new RuntimeException("The index is outside the bounds of the array");
            }
        }
        public void SetValue(ExecutionState s, object v)
        {
            ArrayAccessed[Index] = v;
        }
        public object GetValue(ExecutionState s)
        {
            return ArrayAccessed[Index];
        }
    }
}
