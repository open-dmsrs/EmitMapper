using System.Collections.Generic;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstComplexNode : IAstNode
{
  public List<IAstNode> Nodes = new();

  public void Compile(CompilationContext context)
  {
    foreach (var node in Nodes)
      if (node != null)
        node.Compile(context);
  }
}