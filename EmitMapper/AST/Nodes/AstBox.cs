namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstBox : IAstRef
{
    public IAstRefOrValue Value;

    #region IAstReturnValueNode Members

    public Type ItemType => this.Value.ItemType;

    #endregion

    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        this.Value.Compile(context);

        if (this.Value.ItemType.IsValueType)
            context.Emit(OpCodes.Box, this.ItemType);
    }

    #endregion
}