using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PseudocodeRevisited.ControlStructures;

namespace PseudocodeRevisited
{
    /// <summary>
    /// A <see cref="ControlStructure"/> to which other <see cref="ControlStructure"/>s can be added.
    /// </summary>
    public class ProgramBuilder : ControlStructure
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
    }
}
