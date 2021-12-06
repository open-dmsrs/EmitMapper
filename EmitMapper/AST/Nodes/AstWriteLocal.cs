namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

internal class AstWriteLocal : IAstNode
{
    public int LocalIndex;

    public Type LocalType;

    public IAstRefOrValue Value;

    public AstWriteLocal()
    {
    }

    public AstWriteLocal(LocalBuilder loc, IAstRefOrValue value)
    {
        this.LocalIndex = loc.LocalIndex;
        this.LocalType = loc.LocalType;
        this.Value = value;
    }

    public void Compile(CompilationContext context)
    {
        this.Value.Compile(context);
        CompilationHelper.PrepareValueOnStack(context, this.LocalType, this.Value.ItemType);
        context.Emit(OpCodes.Stloc, this.LocalIndex);
    }
}