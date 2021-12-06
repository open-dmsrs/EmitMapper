using EmitMapper.AST.Interfaces;
using System.Collections.Generic;

namespace EmitMapper.AST.Nodes
{
    internal class AstComplexNode : IAstNode
    {
        public List<IAstNode> Nodes = new List<IAstNode>();

        public void Compile(CompilationContext context)
        {
            foreach (IAstNode node in Nodes)
            {
                if (node != null)
                {
                    node.Compile(context);
                }
            }
        }
    }
}