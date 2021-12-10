namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstExceptionHandlingBlock : IAstNode
{
    private readonly LocalBuilder eceptionVariable;

    private readonly Type exceptionType;

    private readonly IAstNode handlerBlock;

    private readonly IAstNode protectedBlock;

    public AstExceptionHandlingBlock(
        IAstNode protectedBlock,
        IAstNode handlerBlock,
        Type exceptionType,
        LocalBuilder eceptionVariable)
    {
        this.protectedBlock = protectedBlock;
        this.handlerBlock = handlerBlock;
        this.exceptionType = exceptionType;
        this.eceptionVariable = eceptionVariable;
    }

    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        context.ILGenerator.BeginExceptionBlock();
        this.protectedBlock.Compile(context);
        context.ILGenerator.BeginCatchBlock(this.exceptionType);
        context.ILGenerator.Emit(OpCodes.Stloc, this.eceptionVariable);
        this.handlerBlock.Compile(context);
        context.ILGenerator.EndExceptionBlock();
    }

    #endregion
}