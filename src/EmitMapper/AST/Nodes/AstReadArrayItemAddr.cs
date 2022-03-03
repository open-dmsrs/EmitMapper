using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstReadArrayItemAddr : AstReadArrayItem, IAstAddr
{
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
    Array.Compile(context);
    context.Emit(OpCodes.Ldc_I4, Index);
    context.Emit(OpCodes.Ldelema, ItemType);
  }
}