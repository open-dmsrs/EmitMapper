namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstConstantInt32 : IAstValue
{
    public int Value;

    #region IAstReturnValueNode Members

    public Type ItemType => typeof(int);

    #endregion

    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        context.Emit(OpCodes.Ldc_I4, this.Value);
    }

    #endregion
}