using System;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstCastclassRef : AstCastclass, IAstRef
{
  public AstCastclassRef(IAstRefOrValue value, Type targetType)
    : base(value, targetType)
  {
  }

  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    base.Compile(context);
  }
}