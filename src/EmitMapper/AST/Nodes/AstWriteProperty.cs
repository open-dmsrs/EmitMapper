using System;
using System.Collections.Generic;
using System.Reflection;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast write property.
/// </summary>
internal class AstWriteProperty : IAstNode
{
  private readonly PropertyInfo _propertyInfo;

  private readonly MethodInfo _setMethod;

  private readonly IAstRefOrAddr _targetObject;

  private readonly IAstRefOrValue _value;

  /// <summary>
  ///   Initializes a new instance of the <see cref="AstWriteProperty" /> class.
  /// </summary>
  /// <param name="targetObject">The target object.</param>
  /// <param name="value">The value.</param>
  /// <param name="propertyInfo">The property info.</param>
  public AstWriteProperty(IAstRefOrAddr targetObject, IAstRefOrValue value, PropertyInfo propertyInfo)
  {
    _targetObject = targetObject;
    _value = value;
    _propertyInfo = propertyInfo;
    _setMethod = propertyInfo.GetSetMethod();

    if (_setMethod == null)
      throw new Exception("Property " + propertyInfo.Name + " doesn't have set accessor");

    if (_setMethod.GetParameters().Length != 1)
      throw new EmitMapperException("Property " + propertyInfo.Name + " has invalid arguments");
  }

  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    AstBuildHelper.CallMethod(_setMethod, _targetObject, new List<IAstStackItem> { _value }).Compile(context);
  }
}