using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DefaultTokenizer =
    PseudocodeRevisited.Tokenizer<PseudocodeRevisited.ControlStructures.ControlStructParsing.LineType,
        System.Text.RegularExpressions.Match>;

namespace PseudocodeRevisited.ControlStructures
{
    /// <summary>
    /// Contains methods and types used to parse <see cref="ControlStructure"/>s and <see cref="Statement"/>s.
    /// </summary>
    public static class ControlStructParsing
    {
        /// <summary>
        /// A kind of line.
        /// </summary>
        public enum LineType
        {
            If, ElseIf, Else,
            LoopWhile, LoopUntil, LoopFromTo,
            BeginBlock, BeginFunction,
            ReturnNothing, ReturnValue,
            End, Break, Continue,
            Expression,
            Output, Import,
        }
        /// <summary>
        /// Creates a <see cref="DefaultTokenizer"/> to classify, and extract useful information from, lines of code.
        /// </summary>
        public static DefaultTokenizer CreateTokenizer()
        {
            DefaultTokenizer result = new DefaultTokenizer();
            result.Selector = (m) => m;
            result.AddTokenSpec(LineType.If, @"if (?<cond>.+) then$");
            result.AddTokenSpec(LineType.ElseIf, @"else if (?<cond>.+) then$");
            result.AddTokenSpec(LineType.Else, @"else$");
            result.AddTokenSpec(LineType.LoopWhile, @"loop while (?<cond>.+)$");
            result.AddTokenSpec(LineType.LoopUntil, @"loop until (?<cond>.+)$");
            result.AddTokenSpec(LineType.LoopFromTo, 
                @"loop (?<var>[_A-Za-z.][_A-Za-z0-9]*) from (?<lower>.+) to (?<upper>.+)$");
            result.AddTokenSpec(LineType.BeginBlock, @"block$");
            result.AddTokenSpec(LineType.BeginFunction, @"function (?<name>[_A-Za-z.][_A-Za-z0-9]*)\s*\((?<args>.*)\)$");
            result.AddTokenSpec(LineType.ReturnNothing, @"return$");
            result.AddTokenSpec(LineType.ReturnValue, @"return (?<content>.+)$");
            result.AddTokenSpec(LineType.End, @"end (?<kind>.+)$");
            result.AddTokenSpec(LineType.Break, @"break$");
            result.AddTokenSpec(LineType.Continue, @"continue$");
            result.AddTokenSpec(LineType.Output, @"output\s*(?<content>.+)$");
            result.AddTokenSpec(LineType.Import,
                @"import (?<id>[_A-Za-z.][_A-Za-z0-9.]*) from (?<library>.+)$");
            result.AddTokenSpec(LineType.Expression, @".+$");
            return result;
        }
        private static readonly string[] LineTerminators = { "\r\n", "\r", "\n" };
        /// <summary>
        /// Splits a code block into lines.
        /// </summary>
        public static string[] SplitLines(string code)
        {
            return code.Split(LineTerminators, StringSplitOptions.None);
        }
        /// <summary>
        /// Creates a hierarchy of connected <see cref="Statement"/>s from a string representing a block of code.
        /// </summary>
        public static Statement MakeProgram(string code)
        {
            ProgramBuilder builder = new ProgramBuilder(0);
            Statement First = builder.FirstStatement;
            var StatementDecider = CreateTokenizer();
            string[] Lines = SplitLines(code);
            int LineNumber = -1;
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
            return builder.FirstStatement;
        }
    }
}
