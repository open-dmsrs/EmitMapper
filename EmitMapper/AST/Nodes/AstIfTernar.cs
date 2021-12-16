using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstIfTernar : IAstRefOrValue
{
    public IAstRefOrValue Condition;

    public IAstRefOrValue FalseBranch;

    public IAstRefOrValue TrueBranch;

    #region IAstNode Members

    public Type ItemType => TrueBranch.ItemType;

    public AstIfTernar(IAstRefOrValue condition, IAstRefOrValue trueBranch, IAstRefOrValue falseBranch)
    {
        if (trueBranch.ItemType != falseBranch.ItemType)
            throw new EmitMapperException("Types mismatch");

        Condition = condition;
        TrueBranch = trueBranch;
        FalseBranch = falseBranch;
    }

    public void Compile(CompilationContext context)
    {
        var elseLabel = context.ILGenerator.DefineLabel();
        var endIfLabel = context.ILGenerator.DefineLabel();

        Condition.Compile(context);
        context.Emit(OpCodes.Brfalse, elseLabel);

        if (TrueBranch != null)
            TrueBranch.Compile(context);
        if (FalseBranch != null)
            context.Emit(OpCodes.Br, endIfLabel);

        context.ILGenerator.MarkLabel(elseLabel);
        if (FalseBranch != null)
            FalseBranch.Compile(context);
        context.ILGenerator.MarkLabel(endIfLabel);
    }

    #endregion
}