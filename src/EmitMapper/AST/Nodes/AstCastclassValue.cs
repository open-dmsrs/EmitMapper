using System;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstCastclassValue : AstCastclass, IAstValue
{
  public AstCastclassValue(IAstRefOrValue value, Type targetType)
    : base(value, targetType)
  {
  }

  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
    base.Compile(context);
  }
}