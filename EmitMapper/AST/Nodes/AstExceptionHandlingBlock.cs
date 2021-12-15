using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstExceptionHandlingBlock : IAstNode
{
    private readonly IAstNode _handlerBlock;

    private readonly IAstNode _protectedBlock;
    private readonly LocalBuilder _exceptionVariable;
    private readonly Type _exceptionType;

    public AstExceptionHandlingBlock(
        IAstNode protectedBlock,
        IAstNode handlerBlock,
        Type exceptionType,
        LocalBuilder exceptionVariable)
    {
        _protectedBlock = protectedBlock;
        _handlerBlock = handlerBlock;
        _exceptionType = exceptionType;
        _exceptionVariable = exceptionVariable;
    }

    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        context.ILGenerator.BeginExceptionBlock();
        _protectedBlock.Compile(context);
        context.ILGenerator.BeginCatchBlock(_exceptionType);
        context.ILGenerator.Emit(OpCodes.Stloc, _exceptionVariable);
        _handlerBlock.Compile(context);
        context.ILGenerator.EndExceptionBlock();
    }

    #endregion
}