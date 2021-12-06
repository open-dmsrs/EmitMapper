namespace EmitMapper.AST.Nodes;

using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstIf : IAstNode
{
    public IAstValue Condition;

    public AstComplexNode FalseBranch;

    public AstComplexNode TrueBranch;

    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        var elseLabel = context.ILGenerator.DefineLabel();
        var endIfLabel = context.ILGenerator.DefineLabel();

        this.Condition.Compile(context);
        context.Emit(OpCodes.Brfalse, elseLabel);

        if (this.TrueBranch != null)
            this.TrueBranch.Compile(context);
        if (this.FalseBranch != null)
            context.Emit(OpCodes.Br, endIfLabel);

        context.ILGenerator.MarkLabel(elseLabel);
        if (this.FalseBranch != null)
            this.FalseBranch.Compile(context);
        context.ILGenerator.MarkLabel(endIfLabel);
    }

    #endregion
}