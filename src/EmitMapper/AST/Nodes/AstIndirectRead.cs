using System;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal abstract class AstIndirectRead : IAstStackItem
{
  public Type ItemType { get; set; }

  public abstract void Compile(CompilationContext context);
}