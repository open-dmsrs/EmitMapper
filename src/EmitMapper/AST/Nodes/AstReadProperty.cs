using System;
using System.Reflection;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstReadProperty : IAstRefOrValue
{
  public PropertyInfo PropertyInfo;

  public IAstRefOrAddr SourceObject;

  public Type ItemType => PropertyInfo.PropertyType;

  public virtual void Compile(CompilationContext context)
  {
    var mi = PropertyInfo.GetGetMethod();

    if (mi == null)
      throw new Exception("Property " + PropertyInfo.Name + " doesn't have get accessor");

    AstBuildHelper.CallMethod(mi, SourceObject, null).Compile(context);
  }
}