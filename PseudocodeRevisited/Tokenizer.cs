using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PseudocodeRevisited
{
    /// <summary>
    /// A generalized tokenizer which splits a string into tokens defined by regular expressions.
    /// </summary>
    /// <typeparam name="TokenType">The type used to classify tokens.</typeparam>
    /// <typeparam name="TExtra">The type that contains extra data about the token.</typeparam>
    public class Tokenizer<TokenType, TExtra> where TokenType : struct where TExtra : class
    {
        /// <summary>
        /// Stores the TokenTypes and which Regex will match them.
        /// </summary>
        private List<Tuple<TokenType, Regex>> TokenSpec;
        /// <summary>
        /// Gets or sets a function which converts a <see cref="Match"/> into an instance of <see cref="TExtra"/>.
        /// </summary>
        public Func<Match, TExtra> Selector { get; set; }
        public Tokenizer()
        {
            TokenSpec = new List<Tuple<TokenType, Regex>>();
        }
        /// <summary>
        /// Adds a new regex pattern to be tokenized.
        /// </summary>
        public void AddTokenSpec(TokenType tt, string matcher)
        {
            TokenSpec.Add(new Tuple<TokenType, Regex>(tt, new Regex(@"\G(?:" + matcher + ")")));
        }
        /// <summary>
        /// Represents a single token produced by a <see cref="Tokenizer{TokenType, TExtra}"/>, complete
        /// with extra information.
        /// </summary>
        public struct Token
        {
            public TokenType TokenType { get; set; }
            public TExtra Match { get; set; }
            public Token(TokenType tt, TExtra m)
            {
                TokenType = tt;
                Match = m;
            }
        }
        /// <summary>
        /// Returns the sequence of tokens from the specified string.
        /// </summary>
        public IEnumerable<Token> Tokenize(string str)
        {
            int index = 0;
            while (index < str.Length)
            {
                Match candidate = null;
                foreach (var spec in TokenSpec)
                {
                    candidate = spec.Item2.Match(str.Substring(index));
                    if (candidate.Success)
                    {
                        index += candidate.Length;
                        yield return new Token(spec.Item1, Selector?.Invoke(candidate));
                        break;
                    }
                }
                if (candidate == null) break;
                if (!candidate.Success)
                {
                    throw new CompileException("Invalid token: " + str.Substring(index));
                }
            }
        }
    }
}
