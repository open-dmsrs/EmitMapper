using EmitMapper.AST.Interfaces;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstIf : IAstNode
    {
        public IAstValue Condition;
        public AstComplexNode TrueBranch;
        public AstComplexNode FalseBranch;


        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            Label elseLabel = context.ILGenerator.DefineLabel();
            Label endIfLabel = context.ILGenerator.DefineLabel();

            Condition.Compile(context);
            context.Emit(OpCodes.Brfalse, elseLabel);

            if (TrueBranch != null)
            {
                TrueBranch.Compile(context);
            }
            if (FalseBranch != null)
            {
                context.Emit(OpCodes.Br, endIfLabel);
            }

            context.ILGenerator.MarkLabel(elseLabel);
            if (FalseBranch != null)
            {
                FalseBranch.Compile(context);
            }
            context.ILGenerator.MarkLabel(endIfLabel);
        }

        #endregion
    }
}