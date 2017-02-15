using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DefaultTokenizer =
    PseudocodeRevisited.Tokenizer<PseudocodeRevisited.Expressions.ExpressionParsing.TokenType, string>;

namespace PseudocodeRevisited.Expressions {
    /// <summary>
    /// Contains methods and types used to parse <see cref="Expression"/>s.
    /// </summary>
    internal static class ExpressionParsing {
        /// <summary>
        /// A kind of token in an expression.
        /// </summary>
        public enum TokenType {
            OpenParen, CloseParen, OpenIndexer, CloseIndexer,
            Function, Separator, Identifier,
            UnaryPlus, UnaryMinus,
            Times, Divide, Modulo, IntDivide,
            Plus, Minus,
            StringLiteral, IntegerLiteral, RealLiteral,
            Equals, Assignment, NotEquals,
            GreaterOrEqual, LessOrEqual, GreaterThan, LessThan,
            LogicalNot, LogicalAnd, LogicalOr,
            Ignore,
        }
        /// <summary>
        /// Creates a <see cref="DefaultTokenizer"/> that tokenizes expressions.
        /// </summary>
        /// <returns></returns>
        public static DefaultTokenizer CreateTokenizer() {
            DefaultTokenizer result = new DefaultTokenizer();
            result.Selector = (m) => m.Value;
            result.AddTokenSpec(TokenType.OpenParen, @"\(");
            result.AddTokenSpec(TokenType.CloseParen, @"\)");
            result.AddTokenSpec(TokenType.OpenIndexer, @"\[");
            result.AddTokenSpec(TokenType.CloseIndexer, @"\]");
            result.AddTokenSpec(TokenType.Separator, @",");
            result.AddTokenSpec(TokenType.Times, @"\*");
            result.AddTokenSpec(TokenType.Divide, @"/");
            result.AddTokenSpec(TokenType.Plus, @"\+");
            result.AddTokenSpec(TokenType.Minus, @"-");
            result.AddTokenSpec(TokenType.NotEquals, @"!=|≠");
            result.AddTokenSpec(TokenType.Equals, @"=");
            result.AddTokenSpec(TokenType.GreaterOrEqual, @">=");
            result.AddTokenSpec(TokenType.LessOrEqual, @"<=");
            result.AddTokenSpec(TokenType.GreaterThan, @">");
            result.AddTokenSpec(TokenType.LessThan, @"<");
            result.AddTokenSpec(TokenType.StringLiteral, @""".*?""");
            result.AddTokenSpec(TokenType.IntegerLiteral, @"\d+");
            result.AddTokenSpec(TokenType.RealLiteral, @"(?:\d+|\d*\.\d+)(?:[Ee][+-]?\d+)?");
            result.AddTokenSpec(TokenType.LogicalNot, @"NOT\b");
            result.AddTokenSpec(TokenType.LogicalAnd, @"AND\b");
            result.AddTokenSpec(TokenType.LogicalOr, @"OR\b");
            result.AddTokenSpec(TokenType.Modulo, @"mod\b");
            result.AddTokenSpec(TokenType.IntDivide, @"div\b");
            result.AddTokenSpec(TokenType.Identifier, @"[_A-Za-z.][_A-Za-z0-9]*");
            result.AddTokenSpec(TokenType.Ignore, @"\s");
            return result;
        }
        /// <summary>
        /// Gets the precedence of the specified operator.
        /// </summary>
        public static int OperatorPrecedence(TokenType e) {
            switch (e) {
            case TokenType.UnaryPlus:
            case TokenType.UnaryMinus:
                return 8;
            case TokenType.Times:
            case TokenType.Divide:
            case TokenType.Modulo:
            case TokenType.IntDivide:
                return 7;
            case TokenType.Plus:
            case TokenType.Minus:
                return 6;
            case TokenType.GreaterOrEqual:
            case TokenType.LessOrEqual:
            case TokenType.GreaterThan:
            case TokenType.LessThan:
                return 5;
            case TokenType.Equals:
            case TokenType.NotEquals:
                return 4;
            case TokenType.LogicalNot:
                return 3;
            case TokenType.LogicalAnd:
                return 2;
            case TokenType.LogicalOr:
                return 1;
            case TokenType.Assignment:
                return 0;
            default:
                return -1;
            }
        }
        /// <summary>
        /// Gets whether the <see cref="TokenType"/> is a value.
        /// </summary>
        public static bool IsValue(TokenType e) {
            switch (e) {
            case TokenType.IntegerLiteral:
            case TokenType.RealLiteral:
            case TokenType.StringLiteral:
            case TokenType.Identifier:
                return true;
            default:
                return false;
            }
        }
        /// <summary>
        /// Gets whether the <see cref="TokenType"/> is an operator.
        /// </summary>
        public static bool IsOperator(TokenType e) {
            return OperatorPrecedence(e) >= 0;
        }
        /// <summary>
        /// Gets the net number of items the token will add to the stack.
        /// </summary>
        public static int StackBalance(DefaultTokenizer.Token t) {
            var tt = t.TokenType;
            if (tt == TokenType.Assignment)
                return -2;
            else if (IsValue(tt))
                return 1;
            else if (tt == TokenType.UnaryMinus || tt == TokenType.UnaryPlus)
                return 0;
            else if (IsOperator(tt))
                return -1;
            else if (tt == TokenType.Function) {
                int Separator = t.Match.LastIndexOf('@');
                int ArgCount = int.Parse(t.Match.Substring(Separator + 1));
                return 1 - ArgCount;
            } else
                return 0;
        }
        /// <summary>
        /// Resolves ambiguity between plus and unary plus, and between minus and unary minus.
        /// Optionally resolves ambiguity between equals and assignment.
        /// Turns array indexing into an operator.
        /// Removes ignored tokens.
        /// Turns an identifier into a function if it is followed by an open parenthesis.
        /// </summary>
        public static IEnumerable<DefaultTokenizer.Token> Preprocess(bool AddAssignment,
            IEnumerable<DefaultTokenizer.Token> tokens) {
            int level = 0;
            bool wasOperator = false;
            DefaultTokenizer.Token? prevIdentifier = null;
            foreach (DefaultTokenizer.Token srcToken in tokens) {
                if (srcToken.TokenType == TokenType.Ignore) continue;
                if (prevIdentifier.HasValue) {
                    if (srcToken.TokenType == TokenType.OpenParen) {
                        yield return new DefaultTokenizer.Token(TokenType.Function, prevIdentifier.Value.Match);
                    } else {
                        yield return prevIdentifier.Value;
                    }
                }
                prevIdentifier = null;
                switch (srcToken.TokenType) {
                case TokenType.OpenIndexer:
                    yield return new DefaultTokenizer.Token(TokenType.Function, ".__index");
                    yield return new DefaultTokenizer.Token(TokenType.OpenParen, srcToken.Match);
                    level++;
                    break;
                case TokenType.CloseIndexer:
                    yield return new DefaultTokenizer.Token(TokenType.CloseParen, srcToken.Match);
                    level--;
                    break;
                case TokenType.OpenParen:
                    yield return new DefaultTokenizer.Token(TokenType.OpenParen, srcToken.Match);
                    level++;
                    break;
                case TokenType.CloseParen:
                    yield return new DefaultTokenizer.Token(TokenType.CloseParen, srcToken.Match);
                    level--;
                    break;
                case TokenType.Plus:
                    if (wasOperator)
                        yield return new DefaultTokenizer.Token(TokenType.UnaryPlus, srcToken.Match);
                    else
                        yield return new DefaultTokenizer.Token(TokenType.Plus, srcToken.Match);
                    break;
                case TokenType.Minus:
                    if (wasOperator)
                        yield return new DefaultTokenizer.Token(TokenType.UnaryMinus, srcToken.Match);
                    else
                        yield return new DefaultTokenizer.Token(TokenType.Minus, srcToken.Match);
                    break;
                case TokenType.Equals:
                    if (AddAssignment && level == 0) {
                        AddAssignment = false;
                        yield return new DefaultTokenizer.Token(TokenType.Assignment, srcToken.Match);
                    } else {
                        yield return new DefaultTokenizer.Token(TokenType.Equals, srcToken.Match);
                    }
                    break;
                case TokenType.Identifier:
                    prevIdentifier = srcToken;
                    break;
                default:
                    yield return srcToken;
                    break;
                }
                wasOperator = OperatorPrecedence(srcToken.TokenType) > 0;
            }
            if (prevIdentifier.HasValue) {
                yield return prevIdentifier.Value;
            }
        }
        /// <summary>
        /// Converts a sequence of tokens in infix notation to postfix notation using
        /// the shunting-yard algorithm.
        /// </summary>
        public static IEnumerable<DefaultTokenizer.Token> ToRPN(
            IEnumerable<DefaultTokenizer.Token> tokens) {
            Stack<DefaultTokenizer.Token> OpStack = new Stack<DefaultTokenizer.Token>();
            Stack<int> ArgCountStack = new Stack<int>();
            bool prevWasLeftParen = false;
            foreach (DefaultTokenizer.Token srcToken in tokens) {
                if (IsValue(srcToken.TokenType)) {
                    yield return srcToken;
                } else if (srcToken.TokenType == TokenType.Function) {
                    OpStack.Push(srcToken);
                    // Member function?
                    if (srcToken.Match[0] == '.')
                        ArgCountStack.Push(1);
                    else
                        ArgCountStack.Push(0);
                } else if (srcToken.TokenType == TokenType.Separator) {
                    if (ArgCountStack.Count <= 0) {
                        throw new CompileException("Unexpected comma");
                    }
                    ArgCountStack.Push(ArgCountStack.Pop() + 1);
                    while (OpStack.Peek().TokenType != TokenType.OpenParen) {
                        if (OpStack.Count <= 0) {
                            throw new CompileException("Unexpected comma or mismatched parentheses");
                        }
                        yield return OpStack.Pop();
                    }
                } else if (IsOperator(srcToken.TokenType)) {
                    while (true) {
                        TokenType top = OpStack.Count > 0 ? OpStack.Peek().TokenType : TokenType.Ignore;
                        if (IsOperator(top) &&
                            OperatorPrecedence(srcToken.TokenType) <= OperatorPrecedence(top)) {
                            yield return OpStack.Pop();
                        } else {
                            break;
                        }
                    }
                    OpStack.Push(srcToken);
                } else if (srcToken.TokenType == TokenType.OpenParen) {
                    OpStack.Push(srcToken);
                } else if (srcToken.TokenType == TokenType.CloseParen) {
                    while (true) {
                        if (OpStack.Count <= 0) {
                            throw new CompileException("Mismatched parentheses");
                        }
                        TokenType top = OpStack.Peek().TokenType;
                        if (top == TokenType.OpenParen)
                            break;
                        yield return OpStack.Pop();
                    }
                    OpStack.Pop();
                    if (OpStack.Count > 0 && OpStack.Peek().TokenType == TokenType.Function) {
                        int ArgCount = ArgCountStack.Pop();
                        if (!prevWasLeftParen)
                            ArgCount += 1;
                        var FuncToken = OpStack.Pop();
                        yield return new DefaultTokenizer.Token(TokenType.Function,
                            FuncToken.Match + "@" + ArgCount.ToString());
                    }
                }
                prevWasLeftParen = srcToken.TokenType == TokenType.OpenParen;
            }
            while (OpStack.Count > 0) {
                TokenType top = OpStack.Peek().TokenType;
                if (top == TokenType.OpenParen) {
                    throw new CompileException("Mismatched parentheses");
                }
                if (top == TokenType.Function) {
                    throw new CompileException("Expected arguments to function " + OpStack.Peek().Match);
                }
                yield return OpStack.Pop();
            }
        }
        public static int StackBalance(IEnumerable<DefaultTokenizer.Token> rpn) {
            return rpn.Sum(StackBalance);
        }
    }
}
