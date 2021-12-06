namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstInitializeLocalVariable : IAstNode
{
    public int LocalIndex;

    public Type LocalType;

    public AstInitializeLocalVariable()
    {
    }

    public AstInitializeLocalVariable(LocalBuilder loc)
    {
        this.LocalType = loc.LocalType;
        this.LocalIndex = loc.LocalIndex;
    }

    public void Compile(CompilationContext context)
    {
        if (this.LocalType.IsValueType)
        {
            context.Emit(OpCodes.Ldloca, this.LocalIndex);
            context.Emit(OpCodes.Initobj, this.LocalType);
        }
    }
}