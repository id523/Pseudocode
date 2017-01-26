using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited
{
    /// <summary>
    /// Represents an IB "Collection" data structure.
    /// If it is possible to have a least favorite data type, this is mine.
    /// </summary>
    public class Pseudocollection : IEnumerable<object>
    {
        /// <summary>
        /// A List that stores the items in the <see cref="Pseudocollection"/>.
        /// </summary>
        List<object> backing;
        /// <summary>
        /// Where we are in the iteration.
        /// </summary>
        int ptr = 0;
        /// <summary>
        /// Creates a new, empty <see cref="Pseudocollection"/>.
        /// </summary>
        public Pseudocollection()
        {
            backing = new List<object>();
        }
        /// <summary>
        /// Adds an item to the end.
        /// </summary>
        public void AddItem(object obj)
        {
            backing.Add(obj);
        }
        /// <summary>
        /// Checks whether or not there are more items to retrieve with GetNext().
        /// </summary>
        public bool HasNext()
        {
            return ptr < backing.Count;
        }
        /// <summary>
        /// Gets the next item.
        /// </summary>
        public object GetNext()
        {
            if (HasNext())
                return backing[ptr++];
            else
                throw new RuntimeException("There are no more items to iterate through in the collection");
        }
        /// <summary>
        /// Resets the <see cref="Pseudocollection"/> so that the next call to <see cref="GetNext"/> will
        /// return the first item.
        /// </summary>
        public void ResetNext()
        {
            ptr = 0;
        }
        /// <summary>
        /// Checks whether there are any items. This is True after creation and
        /// False after the first call to <see cref="AddItem(object)"/>;
        /// </summary>
        public bool IsEmpty()
        {
            return backing.Count == 0;
        }

        public IEnumerator<object> GetEnumerator()
        {
            return ((IEnumerable<object>)backing).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
