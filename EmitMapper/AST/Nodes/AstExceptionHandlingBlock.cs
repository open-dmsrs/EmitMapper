namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstExceptionHandlingBlock : IAstNode
{
    private readonly LocalBuilder _exceptionVariable;

    private readonly Type _exceptionType;

    private readonly IAstNode _handlerBlock;

    private readonly IAstNode _protectedBlock;

    public AstExceptionHandlingBlock(
        IAstNode protectedBlock,
        IAstNode handlerBlock,
        Type exceptionType,
        LocalBuilder exceptionVariable)
    {
        this._protectedBlock = protectedBlock;
        this._handlerBlock = handlerBlock;
        this._exceptionType = exceptionType;
        this._exceptionVariable = exceptionVariable;
    }

    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        context.ILGenerator.BeginExceptionBlock();
        this._protectedBlock.Compile(context);
        context.ILGenerator.BeginCatchBlock(this._exceptionType);
        context.ILGenerator.Emit(OpCodes.Stloc, this._exceptionVariable);
        this._handlerBlock.Compile(context);
        context.ILGenerator.EndExceptionBlock();
    }

    #endregion
}