using System;
using System.Reflection;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;
/// <summary>
/// The ast read property.
/// </summary>

internal class AstReadProperty : IAstRefOrValue
{
  public PropertyInfo PropertyInfo;

  public IAstRefOrAddr SourceObject;

  /// <summary>
  /// Gets the item type.
  /// </summary>
  public Type ItemType => PropertyInfo.PropertyType;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="context">The context.</param>
  /// <exception cref="Exception"></exception>
  public virtual void Compile(CompilationContext context)
  {
    var mi = PropertyInfo.GetGetMethod();

    if (mi == null)
      throw new Exception("Property " + PropertyInfo.Name + " doesn't have get accessor");

    AstBuildHelper.CallMethod(mi, SourceObject, null).Compile(context);
  }
}