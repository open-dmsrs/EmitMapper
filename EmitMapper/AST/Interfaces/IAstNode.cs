namespace EmitMapper.AST.Interfaces
{
    internal interface IAstNode
    {
        void Compile(CompilationContext context);
    }
}