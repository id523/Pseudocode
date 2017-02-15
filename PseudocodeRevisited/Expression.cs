using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PseudocodeRevisited.Statements;
using PseudocodeRevisited.Expressions;

namespace PseudocodeRevisited {
    /// <summary>
    /// Represents a mathematical expression or a series of function calls.
    /// </summary>
    public sealed class Expression : Statement, IGetValue {
        /// <summary>
        /// The Expression in postfix notation (Reverse Polish Notation).
        /// This requires less parsing than the normal infix form.
        /// </summary>
        Tokenizer<ExpressionParsing.TokenType, string>.Token[] RPNForm;
        /// <summary>
        /// The original text of the Expression, before parsing.
        /// This is used mainly for debugging the interpreter itself.
        /// </summary>
        private string Original;
        /// <summary>
        /// Parses an expression from a string.
        /// </summary>
        /// <param name="s">The string to parse from.</param>
        /// <param name="AddAssign">Whether or not to replace an equals sign with an assignment operator.
        /// In other words, whether or not the Expression is a stand-alone statement.</param>
        /// <param name="IsFunction">The function that determines whether an identifier is a function or not.</param>
        public Expression(int lineNumber, string s, bool AddAssign = false) : base(lineNumber) {
            Original = s;
            var tokenizer = ExpressionParsing.CreateTokenizer();
            var before1 = tokenizer.Tokenize(s);
            var before2 = ExpressionParsing.Preprocess(AddAssign, before1);
            RPNForm = ExpressionParsing.ToRPN(before2).ToArray();
            int balance = ExpressionParsing.StackBalance(RPNForm);
            if (balance < 0) throw new CompileException("Expected literal or variable");
            if (balance > 1) throw new CompileException("Expected operator or function");
        }
        /// <summary>
        /// Pops a value from the value stack, and if it is an instance of <see cref="IGetValue"/>, it will get
        /// the value.
        /// </summary>
        private static object PopValue(Stack<object> EvalStack, ExecutionState s) {
            object popped = EvalStack.Pop();
            IGetValue casted = popped as IGetValue;
            if (casted != null)
                return casted.GetValue(s);
            else
                return popped;
        }
        /// <summary>
        /// Pops a reference to a variable (<see cref="ISetValue"/>) from the stack and tries to assign the
        /// specified value to it. If that fails, it will do an ordinary equality check instead.
        /// </summary>
        private static void PopTryAssign(Stack<object> EvalStack, ExecutionState s, object val) {
            object popped = EvalStack.Pop();
            ISetValue casted = popped as ISetValue;
            if (casted != null) {
                // Set the value
                casted.SetValue(s, val);
            } else {
                // Assignment failed, do equality check
                EvalStack.Push(popped);
                EvalStack.Push(val.Equals(PopValue(EvalStack, s)));
            }
        }
        /// <summary>
        /// Calculates the value of this <see cref="Expression"/>, given the variables in
        /// the specified <see cref="ExecutionState"/>. Also performs any functions in the
        /// <see cref="Expression"/>.
        /// </summary>
        public object GetValue(ExecutionState s) {
            Stack<object> EvalStack = new Stack<object>();
            // Postfix notation can just be read left-to-right
            foreach (var value in RPNForm) {
                object a = null, b = null;
                switch (value.TokenType) {
                case ExpressionParsing.TokenType.Assignment:
                    PopTryAssign(EvalStack, s, PopValue(EvalStack, s));
                    break;
                case ExpressionParsing.TokenType.LogicalNot:
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(Arithmetic.LogicalNot(a));
                    break;
                case ExpressionParsing.TokenType.LogicalAnd:
                    b = PopValue(EvalStack, s);
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(Arithmetic.LogicalAnd(a, b));
                    break;
                case ExpressionParsing.TokenType.LogicalOr:
                    b = PopValue(EvalStack, s);
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(Arithmetic.LogicalOr(a, b));
                    break;
                case ExpressionParsing.TokenType.Equals:
                    b = PopValue(EvalStack, s);
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(a.Equals(b));
                    break;
                case ExpressionParsing.TokenType.NotEquals:
                    b = PopValue(EvalStack, s);
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(!a.Equals(b));
                    break;
                case ExpressionParsing.TokenType.LessThan:
                    b = PopValue(EvalStack, s);
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(Arithmetic.Compare(a, b) < 0);
                    break;
                case ExpressionParsing.TokenType.GreaterThan:
                    b = PopValue(EvalStack, s);
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(Arithmetic.Compare(a, b) > 0);
                    break;
                case ExpressionParsing.TokenType.LessOrEqual:
                    b = PopValue(EvalStack, s);
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(Arithmetic.Compare(a, b) <= 0);
                    break;
                case ExpressionParsing.TokenType.GreaterOrEqual:
                    b = PopValue(EvalStack, s);
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(Arithmetic.Compare(a, b) >= 0);
                    break;
                case ExpressionParsing.TokenType.StringLiteral:
                    EvalStack.Push(value.Match.Substring(1, value.Match.Length - 2));
                    break;
                case ExpressionParsing.TokenType.IntegerLiteral:
                    EvalStack.Push(long.Parse(value.Match));
                    break;
                case ExpressionParsing.TokenType.RealLiteral:
                    EvalStack.Push(double.Parse(value.Match));
                    break;
                case ExpressionParsing.TokenType.Plus:
                    b = PopValue(EvalStack, s);
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(Arithmetic.Add(a, b));
                    break;
                case ExpressionParsing.TokenType.Minus:
                    b = PopValue(EvalStack, s);
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(Arithmetic.Subtract(a, b));
                    break;
                case ExpressionParsing.TokenType.Times:
                    b = PopValue(EvalStack, s);
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(Arithmetic.Multiply(a, b));
                    break;
                case ExpressionParsing.TokenType.Divide:
                    b = PopValue(EvalStack, s);
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(Arithmetic.Divide(a, b));
                    break;
                case ExpressionParsing.TokenType.Modulo:
                    b = PopValue(EvalStack, s);
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(Arithmetic.Modulo(a, b));
                    break;
                case ExpressionParsing.TokenType.IntDivide:
                    b = PopValue(EvalStack, s);
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(Arithmetic.IntDivide(a, b));
                    break;
                case ExpressionParsing.TokenType.UnaryPlus:
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(Arithmetic.Add(0, a));
                    break;
                case ExpressionParsing.TokenType.UnaryMinus:
                    a = PopValue(EvalStack, s);
                    EvalStack.Push(Arithmetic.Subtract(0, a));
                    break;
                case ExpressionParsing.TokenType.Identifier:
                    EvalStack.Push(new SimpleVariable(value.Match));
                    break;
                case ExpressionParsing.TokenType.Function: {
                        int Separator = value.Match.LastIndexOf('@');
                        string FuncName = value.Match.Remove(Separator);
                        int ArgCount = int.Parse(value.Match.Substring(Separator + 1));
                        object[] Arguments = new object[ArgCount];
                        for (int i = ArgCount - 1; i >= 0; i--) {
                            Arguments[i] = PopValue(EvalStack, s);
                        }
                        object RetVal = s.CallFunction(FuncName, Arguments);
                        EvalStack.Push(RetVal);
                    }
                    break;
                }
            }
            if (EvalStack.Count > 0) {
                return PopValue(EvalStack, s);
            } else {
                return null;
            }
        }
        /// <summary>
        /// Performs any functions, assignments, etc. in the <see cref="Expression"/>.
        /// </summary>
        protected override void Run(ExecutionState s) {
            GetValue(s);
        }
    }
}
