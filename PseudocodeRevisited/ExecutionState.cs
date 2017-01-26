using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PseudocodeRevisited.Statements;

namespace PseudocodeRevisited
{
    public delegate object PseudocodeFunction(ExecutionState s, object[] args);
    /// <summary>
    /// Holds all of the data required to run a pseudocode program.
    /// </summary>
    public partial class ExecutionState
    {
        /// <summary>
        /// Creates a new ExecutionState.
        /// </summary>
        public ExecutionState() : this(null) { }
        public ExecutionState(ExecutionState parent)
        {
            Vars = new Scope(parent.Vars);
            LoadedLibraries = parent?.LoadedLibraries ?? new Dictionary<string, Library>();
        }
        /// <summary>
        /// Gets or sets the next statement of the program to run.
        /// </summary>
        public Statement NextStatement { get; set; }
        /// <summary>
        /// Gets a Scope which gives access to the local variables, and the statements for
        /// <see cref="Break"/> and <see cref="Continue"/> to return to.
        /// </summary>
        public Scope Vars { get; private set; }
        /// <summary>
        /// Gets or sets the return value from the function being run in this ExecutionState.
        /// If no function is being run, this value is ignored.
        /// </summary>
        public object ReturnValue { get; set; }
        public void AddFunction(string identifier, PseudocodeFunction action)
        {
            Vars.SetVariable(identifier, action);
        }
        /// <summary>
        /// Calls the function with the specified name and arguments.
        /// </summary>
        public object CallFunction(string identifier, object[] args)
        {
            object funcObj = Vars.GetVariable(identifier);
            return CallFunction(funcObj, args, identifier + " is not a function");
        }
        /// <summary>
        /// Calls an anonymous function with the specified arguments and error message on failure.
        /// </summary>
        public object CallFunction(object funcObj, object[] args,
            string errorMessage = "Unable to call function")
        {
            if (funcObj is PseudocodeFunction)
            {
                return ((PseudocodeFunction)funcObj).Invoke(this, args);
            }
            else
            {
                throw new RuntimeException(errorMessage);
            }
        }
        /// <summary>
        /// Jumps to the end of the last control structure that supports it,
        /// discarding any variables in control structures that were jumped out of.
        /// Returns the <see cref="Statement"/> to jump to.
        /// </summary>
        public Statement Break()
        {
            while (Vars.BreakLocation == null)
            {
                if (Vars.Parent == null)
                    throw new RuntimeException("'break' is not permitted here");
                Vars = Vars.Parent;
            }
            return Vars.BreakLocation;
        }
        /// <summary>
        /// Performs the next iteration in the last control structure that supports that operation,
        /// discarding any variables in control structures that were jumped out of.
        /// Returns the <see cref="Statement"/> to jump to.
        /// </summary>
        public Statement Continue()
        {
            while (Vars.ContinueLocation == null)
            {
                if (Vars.Parent == null)
                    throw new RuntimeException("'continue' is not permitted here");
                Vars = Vars.Parent;
            }
            return Vars.ContinueLocation;
        }
        /// <summary>
        /// Signals the start of a new scope with its own local variables.
        /// </summary>
        public void PushContext()
        {
            Vars = new Scope(Vars);
        }
        /// <summary>
        /// Exits the scope created by the matching <see cref="PushContext"/>, discarding any local variables.
        /// </summary>
        public void PopContext()
        {
            Vars = Vars.Parent ?? Vars;
        }
        /// <summary>
        /// Runs a single statement, and returns False if there are no more statements.
        /// </summary>
        public bool Step()
        {
            if (NextStatement == null)
                return false;
            NextStatement = NextStatement.RunGetNext(this);
            return true;
        }
        /// <summary>
        /// Runs statements until there are no more left.
        /// </summary>
        public void Run()
        {
            while (Step()) ;
        }
    }
}
