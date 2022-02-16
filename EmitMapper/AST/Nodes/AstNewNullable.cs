namespace EmitMapper.AST.Nodes;

using System;

using EmitMapper.AST.Interfaces;

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