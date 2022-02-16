namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection;
using System.Reflection.Emit;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

internal class AstReadLocal : IAstStackItem
{
  public int LocalIndex;

  public Type LocalType;

  public AstReadLocal()
  {
  }

  public AstReadLocal(LocalVariableInfo loc)
  {
    LocalIndex = loc.LocalIndex;
    LocalType = loc.LocalType;
  }

  public Type ItemType => LocalType;

  public virtual void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldloc, LocalIndex);
  }
}

internal class AstReadLocalRef : AstReadLocal, IAstRef
{
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    base.Compile(context);
  }
}

internal class AstReadLocalValue : AstReadLocal, IAstValue
{
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
    base.Compile(context);
  }
}

internal class AstReadLocalAddr : AstReadLocal, IAstAddr
{
  public AstReadLocalAddr(LocalVariableInfo loc)
  {
    LocalIndex = loc.LocalIndex;
    LocalType = loc.LocalType.MakeByRefType();
  }

  public override void Compile(CompilationContext context)
  {
    // CompilationHelper.CheckIsValue(itemType);
    context.Emit(OpCodes.Ldloca, LocalIndex);
  }
}