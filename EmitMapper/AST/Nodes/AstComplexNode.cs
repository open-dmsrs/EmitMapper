namespace EmitMapper.AST.Nodes;

using System.Collections.Generic;

using EmitMapper.AST.Interfaces;

internal class AstComplexNode : IAstNode
{
    public List<IAstNode> Nodes = new();

    public void Compile(CompilationContext context)
    {
        foreach (var node in this.Nodes)
            if (node != null)
                node.Compile(context);
    }
}