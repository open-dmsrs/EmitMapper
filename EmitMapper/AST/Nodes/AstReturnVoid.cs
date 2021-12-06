namespace EmitMapper.AST.Nodes;

using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstReturnVoid : IAstNode
{
    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        context.Emit(OpCodes.Ret);
    }

    #endregion
}