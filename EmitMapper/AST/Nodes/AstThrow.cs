using EmitMapper.AST.Interfaces;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstThrow : IAstNode
    {
        public IAstRef Exception;

        public void Compile(CompilationContext context)
        {
            Exception.Compile(context);
            context.Emit(OpCodes.Throw);
        }
    }
}