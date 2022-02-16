namespace EmitMapper.AST.Nodes;

using System;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

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

internal class AstReadThisRef : AstReadThis, IAstRef
{
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    base.Compile(context);
  }
}

internal class AstReadThisAddr : AstReadThis, IAstRef
{
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    base.Compile(context);
  }
}