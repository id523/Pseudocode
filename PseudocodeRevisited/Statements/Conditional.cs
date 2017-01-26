using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PseudocodeRevisited.Expressions;

namespace PseudocodeRevisited.Statements
{
    /// <summary>
    /// Represents a conditional branching statement.
    /// </summary>
    public sealed class Conditional : Statement
    {
        /// <summary>
        /// The statement which is jumped to if the condition is true.
        /// </summary>
        public Statement TrueBranch { get; private set; }
        /// <summary>
        /// The statement which is jumped to if the condition is false.
        /// </summary>
        public Statement FalseBranch { get { return this; } }
        /// <summary>
        /// The condition to base the control flow decision on.
        /// </summary>
        public Expression Condition { get; private set; }
        /// <summary>
        /// Creates a new <see cref="Conditional"/> with the specified condition.
        /// </summary>
        public Conditional(Expression cond) : base(cond.LineNumber)
        {
            Condition = cond;
            TrueBranch = new Statement(cond.LineNumber);
        }
        protected override Statement RunGetNextInternal(ExecutionState s)
        {
            object EvalCondition = Condition.GetValue(s);
            bool ConvCondition = Convert.ToBoolean(EvalCondition);
            if (ConvCondition)
                return TrueBranch.NormalNext;
            else
                return NormalNext;
        }
    }
}
