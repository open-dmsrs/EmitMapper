namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstConstantString : IAstRef
{
    public string Str;

    #region IAstStackItem Members

    public Type ItemType => typeof(string);

    #endregion

    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        context.Emit(OpCodes.Ldstr, this.Str);
    }

    #endregion
}