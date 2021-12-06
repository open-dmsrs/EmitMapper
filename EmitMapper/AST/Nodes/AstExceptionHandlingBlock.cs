using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstExceptionHandlingBlock : IAstNode
    {
        private readonly IAstNode protectedBlock;
        private readonly IAstNode handlerBlock;
        private readonly Type exceptionType;
        private readonly LocalBuilder eceptionVariable;

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
            Label endBlock = context.ILGenerator.BeginExceptionBlock();
            protectedBlock.Compile(context);
            context.ILGenerator.BeginCatchBlock(exceptionType);
            context.ILGenerator.Emit(OpCodes.Stloc, eceptionVariable);
            handlerBlock.Compile(context);
            context.ILGenerator.EndExceptionBlock();
        }

        #endregion
    }
}