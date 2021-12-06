namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

internal class AstReturn : IAstNode, IAstAddr
{
    public Type ReturnType;

    public IAstRefOrValue ReturnValue;

    public Type ItemType => this.ReturnType;

    public void Compile(CompilationContext context)
    {
        this.ReturnValue.Compile(context);
        CompilationHelper.PrepareValueOnStack(context, this.ReturnType, this.ReturnValue.ItemType);
        context.Emit(OpCodes.Ret);
    }
}