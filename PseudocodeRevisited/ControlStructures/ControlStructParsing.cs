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
    internal static class ControlStructParsing
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
        internal static string[] SplitLines(string code)
        {
            return code.Split(LineTerminators, StringSplitOptions.None);
        }
    }
}
