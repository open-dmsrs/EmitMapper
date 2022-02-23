using System;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstReadThis : IAstRefOrAddr
{
  public Type ThisType;

  public Type ItemType => ThisType;

  public virtual void Compile(CompilationContext context)
  {
    var arg = new AstReadArgument { ArgumentIndex = 0, ArgumentType = ThisType };
    arg.Compile(context);
  }
}