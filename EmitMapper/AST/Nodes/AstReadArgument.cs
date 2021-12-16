using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstReadArgument : IAstStackItem
{
    public int ArgumentIndex;

    public Type ArgumentType;

    public Type ItemType => ArgumentType;

    public virtual void Compile(CompilationContext context)
    {
        switch (ArgumentIndex)
        {
            case 0:
                context.Emit(OpCodes.Ldarg_0);
                break;
            case 1:
                context.Emit(OpCodes.Ldarg_1);
                break;
            case 2:
                context.Emit(OpCodes.Ldarg_2);
                break;
            case 3:
                context.Emit(OpCodes.Ldarg_3);
                break;
            default:
                context.Emit(OpCodes.Ldarg, ArgumentIndex);
                break;
        }
    }
}

internal class AstReadArgumentRef : AstReadArgument, IAstRef
{
    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsRef(ItemType);
        base.Compile(context);
    }
}

internal class AstReadArgumentValue : AstReadArgument, IAstValue
{
    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsValue(ItemType);
        base.Compile(context);
    }
}

internal class AstReadArgumentAddr : AstReadArgument, IAstAddr
{
    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsValue(ItemType);
        context.Emit(OpCodes.Ldarga, ArgumentIndex);
    }
}