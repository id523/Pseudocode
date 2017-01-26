using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PseudocodeRevisited.ControlStructures;
using static PseudocodeRevisited.ControlStructures.ControlStructParsing;

namespace PseudocodeRevisited
{
    /// <summary>
    /// A <see cref="ControlStructure"/> to which other <see cref="ControlStructure"/>s can be added.
    /// </summary>
    public sealed class ProgramBuilder : ControlStructure
    {
        /// <summary>
        /// Stores the nested, unfinished <see cref="ControlStructure"/>s.
        /// </summary>
        private Stack<ControlStructure> ControlStructs = new Stack<ControlStructure>();
        /// <summary>
        /// Creates a new, empty ProgramBuilder.
        /// </summary>
        public ProgramBuilder(int lineNumber)
        {
            Block mainBlock = new Block(lineNumber);
            FirstStatement = mainBlock.FirstStatement;
            ControlStructs.Push(mainBlock);
        }
        /// <summary>
        /// Adds statements to the innermost <see cref="ControlStructure"/>.
        /// </summary>
        public override void AddStatements(Statement first, Statement last)
        {
            ControlStructs.Peek().AddStatements(first, last);
        }
        /// <summary>
        /// Starts an <see cref="IfBlock"/> with the specified condition.
        /// </summary>
        public void AddIf(Expression cond)
        {
            ControlStructs.Push(new IfBlock(cond));
        }
        /// <summary>
        /// Adds another condition to the current <see cref="IfBlock"/>,
        /// or causes an error if there is no if block.
        /// </summary>
        public void AddElseIf(Expression cond)
        {
            IfBlock ifblock = ControlStructs.Peek() as IfBlock;
            if (ifblock == null)
                throw new CompileException("'else if' not valid outside if");
            ifblock.ElseIf(cond);
        }
        /// <summary>
        /// Adds an else-section to the current <see cref="IfBlock"/>,
        /// or causes an error if there is no if block.
        /// </summary>
        public void AddElse()
        {
            IfBlock ifblock = ControlStructs.Peek() as IfBlock;
            if (ifblock == null)
                throw new CompileException("'else' not valid outside if");
            ifblock.Else();
        }
        /// <summary>
        /// Nests a new <see cref="ControlStructure"/> inside the innermost <see cref="ControlStructure"/>.
        /// </summary>
        public void AddControlStructure(ControlStructure c)
        {
            ControlStructs.Push(c);
        }
        /// <summary>
        /// Finishes the innermost <see cref="ControlStructure"/>.
        /// If an incorrect <see cref="ControlStructure.Name"/> is given, a <see cref="CompileException"/> is thrown.
        /// </summary>
        /// <param name="structName">The value to compare to the innermost ControlStructure's Name.</param>
        public void EndControlStructure(int lineNumber, string structName)
        {
            if (ControlStructs.Count <= 1)
            {
                throw new CompileException("Unexpected end");
            }
            ControlStructure ending = ControlStructs.Pop();
            if (ending.Name != structName)
            {
                throw new CompileException(
                    string.Format("'end {0}' is not valid for finishing '{1}'", structName, ending.Name));
            }
            ControlStructure parent = ControlStructs.Peek();
            parent.AddStatements(ending.FirstStatement, ending.Finish(lineNumber));
        }
        /// <summary>
        /// Finishes the ProgramBuilder's list of instructions.
        /// </summary>
        /// <returns></returns>
        protected override Statement FinishProtected(int lineNumber)
        {
            if (ControlStructs.Count > 1)
            {
                throw new CompileException("Expected end " + ControlStructs.Peek().Name);
            }
            return ControlStructs.Pop().Finish(lineNumber);
        }
        public override string Name { get { return null; } }
        /// <summary>
        /// Creates a finished <see cref="ProgramBuilder"/> from a string containing a pseudocode program.
        /// </summary>
        public static ProgramBuilder MakeProgram(string code)
        {
            ProgramBuilder builder = new ProgramBuilder(0);
            Statement First = builder.FirstStatement;
            var StatementDecider = CreateTokenizer();
            string[] Lines = SplitLines(code);
            int LineNumber = 0;
            try
            {
                for (int i = 0; i < Lines.Length; i++)
                {
                    LineNumber = i + 1;
                    // Remove comments
                    int comment_start = Lines[i].IndexOf("//");
                    if (comment_start >= 0)
                    {
                        Lines[i] = Lines[i].Remove(comment_start);
                    }
                    // Remove leading and trailing space
                    Lines[i] = Lines[i].Trim();
                    if (Lines[i].Length == 0)
                        continue;
                    // Decide what sort of statement it is
                    string statement = Lines[i];
                    var decision = StatementDecider.Tokenize(statement).First();
                    string condition;
                    switch (decision.TokenType)
                    {
                        case LineType.If:
                            condition = decision.Match.Groups["cond"].Value;
                            builder.AddIf(new Expression(LineNumber, condition, false));
                            break;
                        case LineType.ElseIf:
                            condition = decision.Match.Groups["cond"].Value;
                            builder.AddElseIf(new Expression(LineNumber, condition, false));
                            break;
                        case LineType.Else:
                            builder.AddElse();
                            break;
                        case LineType.LoopWhile:
                            condition = decision.Match.Groups["cond"].Value;
                            builder.AddControlStructure(
                                new LoopWhile(new Expression(LineNumber, condition, false)));
                            break;
                        case LineType.LoopUntil:
                            condition = decision.Match.Groups["cond"].Value;
                            builder.AddControlStructure(
                                new LoopUntil(new Expression(LineNumber, condition, false)));
                            break;
                        case LineType.LoopFromTo:
                            Expression lower = new Expression(LineNumber, decision.Match.Groups["lower"].Value, false);
                            Expression upper = new Expression(LineNumber, decision.Match.Groups["upper"].Value, false);
                            string varname = decision.Match.Groups["var"].Value;
                            builder.AddControlStructure(new LoopFromTo(varname, lower, upper));
                            break;
                        case LineType.BeginBlock:
                            builder.AddControlStructure(new IsolatedBlock(LineNumber));
                            break;
                        case LineType.BeginFunction:
                            builder.AddControlStructure(new FunctionBlock(LineNumber,
                                decision.Match.Groups["name"].Value,
                                decision.Match.Groups["args"].Value));
                            break;
                        case LineType.ReturnNothing:
                            builder.AddStatement(new Statements.ReturnNothing(LineNumber));
                            break;
                        case LineType.ReturnValue:
                            Expression retval = new Expression(LineNumber, decision.Match.Groups["content"].Value, false);
                            builder.AddStatement(new Statements.Return(LineNumber, retval));
                            break;
                        case LineType.End:
                            string kind = decision.Match.Groups["kind"].Value;
                            builder.EndControlStructure(LineNumber, kind);
                            break;
                        case LineType.Break:
                            builder.AddStatement(new Statements.Break(LineNumber));
                            break;
                        case LineType.Continue:
                            builder.AddStatement(new Statements.Continue(LineNumber));
                            break;
                        case LineType.Output:
                            string content = decision.Match.Groups["content"].Value;
                            builder.AddStatement(new Expression(LineNumber, "writeLine(" + content + ")", false));
                            break;
                        case LineType.Import:
                            builder.AddStatement(new Statements.Import(LineNumber,
                                decision.Match.Groups["library"].Value,
                                decision.Match.Groups["id"].Value));
                            break;
                        case LineType.Expression:
                            builder.AddStatement(new Expression(LineNumber, decision.Match.Value, true));
                            break;
                    }

                }
                LineNumber = Lines.Length + 1;
                builder.Finish(LineNumber);
            }
            catch (CompileException ex)
            {
                throw new CompileException(LineNumber.ToString() + ": " + ex.Message);
            }
            return builder;
        }
    }
}
