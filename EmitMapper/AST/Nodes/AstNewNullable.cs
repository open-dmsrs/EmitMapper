using EmitMapper.AST.Interfaces;
using System;

namespace EmitMapper.AST.Nodes
{

    /* Unmerged change from project 'EmitMapper (netstandard2.1)'
    Before:
        class AstNewNullable: IAstValue
    After:
        class AstNewNullable: IAstValue
    */
    internal class AstNewNullable : IAstValue
    {
        private readonly Type nullableType;

        public AstNewNullable(Type nullableType)
        {
            this.nullableType = nullableType;
        }

        public Type ItemType => nullableType;
        public void Compile(CompilationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
