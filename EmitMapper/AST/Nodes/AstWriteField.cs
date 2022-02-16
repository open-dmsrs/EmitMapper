namespace EmitMapper.AST.Nodes;

using System.Reflection;
using System.Reflection.Emit;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

internal class AstWriteField : IAstNode
{
  public FieldInfo FieldInfo;

  public IAstRefOrAddr TargetObject;

  public IAstRefOrValue Value;

  public void Compile(CompilationContext context)
  {
    TargetObject.Compile(context);
    Value.Compile(context);
    CompilationHelper.PrepareValueOnStack(context, FieldInfo.FieldType, Value.ItemType);
    context.Emit(OpCodes.Stfld, FieldInfo);
  }
}