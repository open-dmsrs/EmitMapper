using System;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

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