using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PseudocodeRevisited.Statements
{
    /// <summary>
    /// Imports a <see cref="PseudocodeRevisited.Library"/>.
    /// </summary>
    public class Import : Statement
    {
        public string LibraryName { get; private set; }
        public string ID { get; private set; }
        public Import(int lineNumber, string lib, string id) : base(lineNumber)
        {
            LibraryName = lib;
            ID = id;
        }
        protected override void Run(ExecutionState s)
        {
            Library lib = s.LoadLibrary(LibraryName);
            if (!lib.CanImport(ID))
                throw new RuntimeException("Unable to import " + ID);
            lib.Import(ID, s);
        }
    }
}
