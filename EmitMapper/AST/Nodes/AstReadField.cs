namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection;
using System.Reflection.Emit;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

internal class AstReadField : IAstStackItem
{
  public FieldInfo FieldInfo;

  public IAstRefOrAddr SourceObject;

  public Type ItemType => FieldInfo.FieldType;

  public virtual void Compile(CompilationContext context)
  {
    SourceObject.Compile(context);
    context.Emit(OpCodes.Ldfld, FieldInfo);
  }
}

internal class AstReadFieldRef : AstReadField, IAstRef
{
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    base.Compile(context);
  }
}

internal class AstReadFieldValue : AstReadField, IAstValue
{
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
    base.Compile(context);
  }
}

internal class AstReadFieldAddr : AstReadField, IAstAddr
{
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
    SourceObject.Compile(context);
    context.Emit(OpCodes.Ldflda, FieldInfo);
  }
}