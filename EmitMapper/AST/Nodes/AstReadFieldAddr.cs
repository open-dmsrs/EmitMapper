using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstReadFieldAddr : AstReadField, IAstAddr
{
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
    SourceObject.Compile(context);
    context.Emit(OpCodes.Ldflda, FieldInfo);
  }
}