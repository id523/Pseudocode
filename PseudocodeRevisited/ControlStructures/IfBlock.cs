using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conditional = PseudocodeRevisited.Statements.Conditional;

namespace PseudocodeRevisited.ControlStructures
{
    /// <summary>
    /// Represents an if-block, which starts "if [condition]" and ends "end if".
    /// </summary>
    public class IfBlock : ControlStructure
    {
        /// <summary>
        /// The conditional that represents the last "else if [condition]" added.
        /// </summary>
        private Conditional LastConditional;
        /// <summary>
        /// The last statements of the section for each else-if or else.
        /// </summary>
        private List<Statement> LastStatements;
        /// <summary>
        /// Stores whether there is an else section.
        /// (or equivalently, whether statements will be added to the else section).
        /// </summary>
        private bool AddingElse = false;
        /// <summary>
        /// Creates a new if-block with the specified condition.
        /// </summary>
        public IfBlock(Expression cond)
        {
            LastStatements = new List<Statement>();
            FirstStatement = new Statements.PushContext(cond.LineNumber);
            LastConditional = new Conditional(cond);
            FirstStatement.NormalNext = LastConditional;
            LastStatements.Add(LastConditional.TrueBranch);
        }
        /// <summary>
        /// Adds statements to the if-block.
        /// </summary>
        public override void AddStatements(Statement first, Statement last)
        {
            int lastIndex = LastStatements.Count - 1;
            LastStatements[lastIndex].NormalNext = first;
            LastStatements[lastIndex] = last;
        }
        /// <summary>
        /// Adds a new condition to the if-block.
        /// </summary>
        public void ElseIf(Expression cond)
        {
            if (!AddingElse)
            {
                Conditional NextConditional = new Conditional(cond);
                LastConditional.FalseBranch.NormalNext = NextConditional;
                LastStatements.Add(NextConditional.TrueBranch);
                LastConditional = NextConditional;
            }
            else
                throw new CompileException("'else if' is not permitted after 'else'");
        }
        /// <summary>
        /// Adds an else section to the if-block.
        /// </summary>
        public void Else()
        {
            if (!AddingElse)
            {
                LastStatements.Add(LastConditional.FalseBranch);
                AddingElse = true;
            }
            else
                throw new CompileException("'else' is not permitted after 'else'");
        }
        /// <summary>
        /// Finishes the if-block.
        /// </summary>
        protected override Statement FinishProtected(int lineNumber)
        {
            Statement final = new Statements.PopContext(lineNumber);
            foreach (Statement penult in LastStatements)
            {
                penult.NormalNext = final;
            }
            // If there was no else clause, finish the last if statement.
            if (!AddingElse) LastConditional.FalseBranch.NormalNext = final;
            return final;
        }
        public override string Name { get { return "if"; } }
    }
}
