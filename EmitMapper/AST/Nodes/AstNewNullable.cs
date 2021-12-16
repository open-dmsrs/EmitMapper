using System;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstNewNullable : IAstValue
{
    public AstNewNullable(Type nullableType)
    {
        ItemType = nullableType;
    }

    public Type ItemType { get; }

    public void Compile(CompilationContext context)
    {
        throw new NotImplementedException();
    }
}