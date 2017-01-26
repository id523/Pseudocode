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
        public Library Library { get; private set; }
        public string ID { get; private set; }
        public Import(int lineNumber, Library lib, string id) : base(lineNumber)
        {
            Library = lib;
            ID = id;
        }
        protected override void Run(ExecutionState s)
        {
            if (!Library.CanImport(ID))
                throw new RuntimeException("Unable to import " + ID);
            Library.Import(ID, s);
        }
    }
}
