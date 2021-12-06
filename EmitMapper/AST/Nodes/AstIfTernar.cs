namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstIfTernar : IAstRefOrValue
{
    public IAstRefOrValue Condition;

    public IAstRefOrValue FalseBranch;

    public IAstRefOrValue TrueBranch;

    #region IAstNode Members

    public Type ItemType => this.TrueBranch.ItemType;

    public AstIfTernar(IAstRefOrValue condition, IAstRefOrValue trueBranch, IAstRefOrValue falseBranch)
    {
        if (trueBranch.ItemType != falseBranch.ItemType)
            throw new EmitMapperException("Types mismatch");

        this.Condition = condition;
        this.TrueBranch = trueBranch;
        this.FalseBranch = falseBranch;
    }

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