using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited.Statements {
    /// <summary>
    /// A statement that defines a function.
    /// </summary>
    public sealed class DefineFunction : Statement {
        /// <summary>
        /// The name of the function.
        /// </summary>
        public string FunctionName { get; private set; }
        /// <summary>
        /// The initial <see cref="Statement"/> of the function.
        /// </summary>
        public Statement FunctionStart { get; set; }
        /// <summary>
        /// The names of the arguments.
        /// </summary>
        private string[] argnames;
        /// <summary>
        /// Creates a new <see cref="DefineFunction"/> with the specified function name,
        /// comma-separated argument name list, and ExecutionState to put the function in.
        /// </summary>
        public DefineFunction(int lineNumber, string funcname, string args) : base(lineNumber) {
            FunctionName = funcname;
            Tokenizer<int, string> argparse = new Tokenizer<int, string>();
            argparse.Selector = (m) => m.Groups["argname"].Value;
            argparse.AddTokenSpec(0, @"\s*(?<argname>[_A-Za-z][_A-Za-z0-9]*)\s*,?");
            argnames = argparse.Tokenize(args).Select((a) => a.Match).ToArray();
        }
        /// <summary>
        /// Defines the function in the specified ExecutionState.
        /// </summary>
        protected override void Run(ExecutionState s) {
            ExecutionState Capture = s;
            s.AddFunction(FunctionName, delegate (ExecutionState s2, object[] args) {
                if (args.Length < argnames.Length)
                    throw new RuntimeException(
                        string.Format("Function {0}() expects at least {1} arguments",
                        FunctionName, argnames.Length));
                ExecutionState funcstate = new ExecutionState(Capture);
                funcstate.Vars.SetVariable("__args", args);
                for (int i = 0; i < argnames.Length; i++) {
                    funcstate.Vars.SetVariable(argnames[i], args[i]);
                }
                funcstate.NextStatement = FunctionStart;
                funcstate.Run();
                return funcstate.ReturnValue;
            });
        }
    }
}
