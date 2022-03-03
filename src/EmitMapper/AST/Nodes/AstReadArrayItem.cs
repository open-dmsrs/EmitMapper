using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstReadArrayItem : IAstStackItem
{
  public IAstRef Array;

  public int Index;

  public Type ItemType => Array.ItemType.GetElementType();

  public virtual void Compile(CompilationContext context)
  {
    Array.Compile(context);
    context.Emit(OpCodes.Ldc_I4, Index);
    context.Emit(OpCodes.Ldelem, ItemType);
  }
}