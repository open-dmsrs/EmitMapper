using System;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

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