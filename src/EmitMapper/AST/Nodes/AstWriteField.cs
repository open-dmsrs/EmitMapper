using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast write field.
/// </summary>
internal class AstWriteField : IAstNode
{
  public FieldInfo FieldInfo;

  public IAstRefOrAddr TargetObject;

  public IAstRefOrValue Value;

  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    TargetObject.Compile(context);
    Value.Compile(context);
    CompilationHelper.PrepareValueOnStack(context, FieldInfo.FieldType, Value.ItemType);
    context.Emit(OpCodes.Stfld, FieldInfo);
  }
}