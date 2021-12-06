namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstConstantNull : IAstRefOrValue
{
    #region IAstReturnValueNode Members

    public Type ItemType => typeof(object);

    #endregion

    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        context.Emit(OpCodes.Ldnull);
    }

    #endregion
}