namespace EmitMapper.AST.Helpers;

/// <summary>
///   The ast build helper.
/// </summary>
internal static class AstBuildHelper
{
  /// <summary>
  ///   Calls the method.
  /// </summary>
  /// <param name="methodInfo">The method info.</param>
  /// <param name="invocationObject">The invocation object.</param>
  /// <param name="arguments">The arguments.</param>
  /// <returns>An IAstRefOrValue.</returns>
  public static IAstRefOrValue CallMethod(
    MethodInfo methodInfo,
    IAstRefOrAddr invocationObject,
    List<IAstStackItem> arguments)
  {
    if (methodInfo.ReturnType.IsValueType)
      return new AstCallMethodValue(methodInfo, invocationObject, arguments);

    return new AstCallMethodRef(methodInfo, invocationObject, arguments);
  }

  /// <summary>
  ///   Casts the class.
  /// </summary>
  /// <param name="value">The value.</param>
  /// <param name="targetType">The target type.</param>
  /// <returns>An IAstRefOrValue.</returns>
  public static IAstRefOrValue CastClass(IAstRefOrValue value, Type targetType)
  {
    if (targetType.IsValueType)
      return new AstCastclassValue(value, targetType);

    return new AstCastclassRef(value, targetType);
  }

  /// <summary>
  ///   Reads the argument r a.
  /// </summary>
  /// <param name="argumentIndex">The argument index.</param>
  /// <param name="argumentType">The argument type.</param>
  /// <returns>An IAstRefOrAddr.</returns>
  public static IAstRefOrAddr ReadArgumentRA(int argumentIndex, Type argumentType)
  {
    if (argumentType.IsValueType)
      return new AstReadArgumentAddr { ArgumentIndex = argumentIndex, ArgumentType = argumentType };

    return new AstReadArgumentRef { ArgumentIndex = argumentIndex, ArgumentType = argumentType };
  }

  /// <summary>
  ///   Reads the argument r v.
  /// </summary>
  /// <param name="argumentIndex">The argument index.</param>
  /// <param name="argumentType">The argument type.</param>
  /// <returns>An IAstRefOrValue.</returns>
  public static IAstRefOrValue ReadArgumentRV(int argumentIndex, Type argumentType)
  {
    if (argumentType.IsValueType)
      return new AstReadArgumentValue { ArgumentIndex = argumentIndex, ArgumentType = argumentType };

    return new AstReadArgumentRef { ArgumentIndex = argumentIndex, ArgumentType = argumentType };
  }

  /// <summary>
  ///   Reads the array item r a.
  /// </summary>
  /// <param name="array">The array.</param>
  /// <param name="index">The index.</param>
  /// <returns>An IAstRefOrAddr.</returns>
  public static IAstRefOrAddr ReadArrayItemRA(IAstRef array, int index)
  {
    if (array.ItemType.IsValueType)
      return new AstReadArrayItemAddr { Array = array, Index = index };

    return new AstReadArrayItemRef { Array = array, Index = index };
  }

  /// <summary>
  ///   Reads the array item r v.
  /// </summary>
  /// <param name="array">The array.</param>
  /// <param name="index">The index.</param>
  /// <returns>An IAstRefOrValue.</returns>
  public static IAstRefOrValue ReadArrayItemRV(IAstRef array, int index)
  {
    if (array.ItemType.IsValueType)
      return new AstReadArrayItemValue { Array = array, Index = index };

    return new AstReadArrayItemRef { Array = array, Index = index };
  }

  /// <summary>
  ///   Reads the field r a.
  /// </summary>
  /// <param name="sourceObject">The source object.</param>
  /// <param name="fieldInfo">The field info.</param>
  /// <returns>An IAstRefOrAddr.</returns>
  public static IAstRefOrAddr ReadFieldRA(IAstRefOrAddr sourceObject, FieldInfo fieldInfo)
  {
    if (fieldInfo.FieldType.IsValueType)
      return new AstReadFieldAddr { FieldInfo = fieldInfo, SourceObject = sourceObject };

    return new AstReadFieldRef { FieldInfo = fieldInfo, SourceObject = sourceObject };
  }

  /// <summary>
  ///   Reads the field r v.
  /// </summary>
  /// <param name="sourceObject">The source object.</param>
  /// <param name="fieldInfo">The field info.</param>
  /// <returns>An IAstRefOrValue.</returns>
  public static IAstRefOrValue ReadFieldRV(IAstRefOrAddr sourceObject, FieldInfo fieldInfo)
  {
    if (fieldInfo.FieldType.IsValueType)
      return new AstReadFieldValue { FieldInfo = fieldInfo, SourceObject = sourceObject };

    return new AstReadFieldRef { FieldInfo = fieldInfo, SourceObject = sourceObject };
  }

  /// <summary>
  ///   Reads the local r a.
  /// </summary>
  /// <param name="loc">The loc.</param>
  /// <returns>An IAstRefOrAddr.</returns>
  public static IAstRefOrAddr ReadLocalRA(LocalBuilder loc)
  {
    if (loc.LocalType.IsValueType)
      return new AstReadLocalAddr(loc);

    return new AstReadLocalRef { LocalType = loc.LocalType, LocalIndex = loc.LocalIndex };
  }

  /// <summary>
  ///   Reads the local r v.
  /// </summary>
  /// <param name="loc">The loc.</param>
  /// <returns>An IAstRefOrValue.</returns>
  public static IAstRefOrValue ReadLocalRV(LocalBuilder loc)
  {
    if (loc.LocalType.IsValueType)
      return new AstReadLocalValue { LocalType = loc.LocalType, LocalIndex = loc.LocalIndex };

    return new AstReadLocalRef { LocalType = loc.LocalType, LocalIndex = loc.LocalIndex };
  }

  /// <summary>
  ///   Reads the member.
  /// </summary>
  /// <param name="sourceObject">The source object.</param>
  /// <param name="memberInfo">The member info.</param>
  /// <exception cref="EmitMapperException"></exception>
  /// <returns>An IAstStackItem.</returns>
  public static IAstStackItem ReadMember(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
  {
    if (memberInfo.MemberType == MemberTypes.Method)
    {
      var methodInfo = memberInfo.DeclaringType.GetMethodCache(memberInfo.Name);

      if (methodInfo.ReturnType == null)
        throw new EmitMapperException("Invalid member:" + memberInfo.Name);

      if (methodInfo.GetParameters().Length > 0)
        throw new EmitMapperException("Method " + memberInfo.Name + " should not have parameters");

      return (IAstRef)CallMethod(methodInfo, sourceObject, null);
    }

    if (memberInfo.MemberType == MemberTypes.Field)
      return ReadFieldRA(sourceObject, (FieldInfo)memberInfo);

    return (IAstRef)ReadPropertyRV(sourceObject, (PropertyInfo)memberInfo);
  }

  /// <summary>
  ///   Reads the member r a.
  /// </summary>
  /// <param name="sourceObject">The source object.</param>
  /// <param name="memberInfo">The member info.</param>
  /// <exception cref="EmitMapperException"></exception>
  /// <returns>An IAstRefOrAddr.</returns>
  public static IAstRefOrAddr ReadMemberRA(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
  {
    if (memberInfo.MemberType == MemberTypes.Method)
    {
      var methodInfo = memberInfo.DeclaringType.GetMethodCache(memberInfo.Name);

      if (methodInfo.ReturnType == null)
        throw new EmitMapperException("Invalid member:" + memberInfo.Name);

      if (methodInfo.GetParameters().Length > 0)
        throw new EmitMapperException("Method " + memberInfo.Name + " should not have parameters");

      if (methodInfo.ReturnType == null || methodInfo.ReturnType.IsValueType)
        throw new EmitMapperException("Method " + memberInfo.Name + " should return a reference");

      return (IAstRef)CallMethod(methodInfo, sourceObject, null);
    }

    if (memberInfo.MemberType == MemberTypes.Field)
      return ReadFieldRA(sourceObject, (FieldInfo)memberInfo);

    var pi = (PropertyInfo)memberInfo;

    if (pi.PropertyType.IsValueType)
      return ReadPropertyRA(sourceObject, (PropertyInfo)memberInfo);

    return (IAstRef)ReadPropertyRV(sourceObject, (PropertyInfo)memberInfo);
  }

  /// <summary>
  ///   Reads the member r v.
  /// </summary>
  /// <param name="sourceObject">The source object.</param>
  /// <param name="memberInfo">The member info.</param>
  /// <exception cref="EmitMapperException"></exception>
  /// <returns>An IAstRefOrValue.</returns>
  public static IAstRefOrValue ReadMemberRV(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
  {
    if (memberInfo.MemberType == MemberTypes.Method)
    {
      var methodInfo = memberInfo.DeclaringType.GetMethodCache(memberInfo.Name);

      if (methodInfo.ReturnType == null)
        throw new EmitMapperException("Invalid member:" + memberInfo.Name);

      if (methodInfo.GetParameters().Length > 0)
        throw new EmitMapperException("Method " + memberInfo.Name + " should not have parameters");

      return CallMethod(methodInfo, sourceObject, null);
    }

    if (memberInfo.MemberType == MemberTypes.Field)
      return ReadFieldRV(sourceObject, (FieldInfo)memberInfo);

    return ReadPropertyRV(sourceObject, (PropertyInfo)memberInfo);
  }

  /// <summary>
  ///   Reads the members chain.
  /// </summary>
  /// <param name="sourceObject">The source object.</param>
  /// <param name="membersChain">The members chain.</param>
  /// <returns>An IAstRefOrValue.</returns>
  public static IAstRefOrValue ReadMembersChain(IAstRefOrAddr sourceObject, IEnumerable<MemberInfo> membersChain)
  {
    var src = sourceObject;
    using var enumerator = membersChain.GetEnumerator();
    MemberInfo cur = null;

    if (enumerator.MoveNext())
      cur = enumerator.Current;

    while (enumerator.MoveNext())
    {
      src = ReadMemberRA(src, cur);
      cur = enumerator.Current;
    }

    return ReadMemberRV(src, cur);
  }

  /// <summary>
  ///   Reads the members chain.
  /// </summary>
  /// <param name="sourceObject">The source object.</param>
  /// <param name="membersChainOfOne">The members chain of one.</param>
  /// <returns>An IAstRefOrValue.</returns>
  public static IAstRefOrValue ReadMembersChain(IAstRefOrAddr sourceObject, MemberInfo membersChainOfOne)
  {
    return ReadMemberRV(sourceObject, membersChainOfOne);
  }

  /// <summary>
  ///   Reads the property r a.
  /// </summary>
  /// <param name="sourceObject">The source object.</param>
  /// <param name="propertyInfo">The property info.</param>
  /// <returns>An IAstRefOrAddr.</returns>
  public static IAstRefOrAddr ReadPropertyRA(IAstRefOrAddr sourceObject, PropertyInfo propertyInfo)
  {
    if (propertyInfo.PropertyType.IsValueType)
      return new AstValueToAddr(new AstReadPropertyValue { SourceObject = sourceObject, PropertyInfo = propertyInfo });

    return new AstReadPropertyRef { SourceObject = sourceObject, PropertyInfo = propertyInfo };
  }

  /// <summary>
  ///   Reads the property r v.
  /// </summary>
  /// <param name="sourceObject">The source object.</param>
  /// <param name="propertyInfo">The property info.</param>
  /// <returns>An IAstRefOrValue.</returns>
  public static IAstRefOrValue ReadPropertyRV(IAstRefOrAddr sourceObject, PropertyInfo propertyInfo)
  {
    if (propertyInfo.PropertyType.IsValueType)
      return new AstReadPropertyValue { SourceObject = sourceObject, PropertyInfo = propertyInfo };

    return new AstReadPropertyRef { SourceObject = sourceObject, PropertyInfo = propertyInfo };
  }

  /// <summary>
  ///   Reads the this.
  /// </summary>
  /// <param name="thisType">The this type.</param>
  /// <returns>An IAstRefOrAddr.</returns>
  public static IAstRefOrAddr ReadThis(Type thisType)
  {
    if (thisType.IsValueType)
      return new AstReadThisAddr { ThisType = thisType };

    return new AstReadThisRef { ThisType = thisType };
  }

  /// <summary>
  ///   Writes the member.
  /// </summary>
  /// <param name="memberInfo">The member info.</param>
  /// <param name="targetObject">The target object.</param>
  /// <param name="value">The value.</param>
  /// <returns>An IAstNode.</returns>
  public static IAstNode WriteMember(MemberInfo memberInfo, IAstRefOrAddr targetObject, IAstRefOrValue value)
  {
    if (memberInfo.MemberType == MemberTypes.Field)
      return new AstWriteField { FieldInfo = (FieldInfo)memberInfo, TargetObject = targetObject, Value = value };

    return new AstWriteProperty(targetObject, value, (PropertyInfo)memberInfo);
  }

  /// <summary>
  ///   Writes the members chain.
  /// </summary>
  /// <param name="membersChain">The members chain.</param>
  /// <param name="targetObject">The target object.</param>
  /// <param name="value">The value.</param>
  /// <returns>An IAstNode.</returns>
  public static IAstNode WriteMembersChain(
    IEnumerable<MemberInfo> membersChain,
    IAstRefOrAddr targetObject,
    IAstRefOrValue value)
  {
    var readTarget = targetObject;
    var enumerator = membersChain.GetEnumerator();
    MemberInfo cur = null;

    if (enumerator.MoveNext())
      cur = enumerator.Current;

    while (enumerator.MoveNext())
    {
      readTarget = ReadMemberRA(readTarget, cur);
      cur = enumerator.Current;
    }

    return WriteMember(cur, readTarget, value);
  }
}