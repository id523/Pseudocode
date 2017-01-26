using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited.Expressions
{
    /// <summary>
    /// An object from which a value can be retrieved.
    /// </summary>
    public interface IGetValue
    {
        object GetValue(ExecutionState s);
    }
    /// <summary>
    /// An object to which a value can be stored.
    /// </summary>
    public interface ISetValue
    {
        void SetValue(ExecutionState s, object v);
    }
    /// <summary>
    /// An object which can have its value read or written.
    /// </summary>
    public interface IFullValue : IGetValue, ISetValue { }
}
