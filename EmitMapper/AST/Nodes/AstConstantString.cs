using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstConstantString : IAstRef
{
    public string Str;

    #region IAstStackItem Members

    public Type ItemType => Meta<String>.Type;

    #endregion

    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        context.Emit(OpCodes.Ldstr, Str);
    }

    #endregion
}