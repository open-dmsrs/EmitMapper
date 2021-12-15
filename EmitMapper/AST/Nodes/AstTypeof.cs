namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstTypeof : IAstRef
{
    public Type Type;

    #region IAstStackItem Members

    public Type ItemType => typeof(Type);

    #endregion

    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        context.Emit(OpCodes.Ldtoken, this.Type);
        context.EmitCall(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)));
    }

    #endregion
}