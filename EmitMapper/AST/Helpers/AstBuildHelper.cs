using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;

namespace EmitMapper.AST.Helpers;

internal static class AstBuildHelper
{
  public static IAstRefOrValue CallMethod(
    MethodInfo methodInfo,
    IAstRefOrAddr invocationObject,
    List<IAstStackItem> arguments)
  {
    if (methodInfo.ReturnType.IsValueType)
      return new AstCallMethodValue(methodInfo, invocationObject, arguments);
    return new AstCallMethodRef(methodInfo, invocationObject, arguments);
  }

  public static IAstRefOrValue ReadArgumentRV(int argumentIndex, Type argumentType)
  {
    if (argumentType.IsValueType)
      return new AstReadArgumentValue { ArgumentIndex = argumentIndex, ArgumentType = argumentType };
    return new AstReadArgumentRef { ArgumentIndex = argumentIndex, ArgumentType = argumentType };
  }

  public static IAstRefOrAddr ReadArgumentRA(int argumentIndex, Type argumentType)
  {
    if (argumentType.IsValueType)
      return new AstReadArgumentAddr { ArgumentIndex = argumentIndex, ArgumentType = argumentType };
    return new AstReadArgumentRef { ArgumentIndex = argumentIndex, ArgumentType = argumentType };
  }

  public static IAstRefOrValue ReadArrayItemRV(IAstRef array, int index)
  {
    if (array.ItemType.IsValueType)
      return new AstReadArrayItemValue { Array = array, Index = index };
    return new AstReadArrayItemRef { Array = array, Index = index };
  }

  public static IAstRefOrAddr ReadArrayItemRA(IAstRef array, int index)
  {
    if (array.ItemType.IsValueType)
      return new AstReadArrayItemAddr { Array = array, Index = index };
    return new AstReadArrayItemRef { Array = array, Index = index };
  }

  public static IAstRefOrValue ReadFieldRV(IAstRefOrAddr sourceObject, FieldInfo fieldInfo)
  {
    if (fieldInfo.FieldType.IsValueType)
      return new AstReadFieldValue { FieldInfo = fieldInfo, SourceObject = sourceObject };
    return new AstReadFieldRef { FieldInfo = fieldInfo, SourceObject = sourceObject };
  }

  public static IAstRefOrValue CastClass(IAstRefOrValue value, Type targetType)
  {
    if (targetType.IsValueType)
      return new AstCastclassValue(value, targetType);
    return new AstCastclassRef(value, targetType);
  }

  public static IAstRefOrAddr ReadFieldRA(IAstRefOrAddr sourceObject, FieldInfo fieldInfo)
  {
    if (fieldInfo.FieldType.IsValueType)
      return new AstReadFieldAddr { FieldInfo = fieldInfo, SourceObject = sourceObject };
    return new AstReadFieldRef { FieldInfo = fieldInfo, SourceObject = sourceObject };
  }

  public static IAstRefOrValue ReadLocalRV(LocalBuilder loc)
  {
    if (loc.LocalType.IsValueType)
      return new AstReadLocalValue { LocalType = loc.LocalType, LocalIndex = loc.LocalIndex };
    return new AstReadLocalRef { LocalType = loc.LocalType, LocalIndex = loc.LocalIndex };
  }

  public static IAstRefOrAddr ReadLocalRA(LocalBuilder loc)
  {
    if (loc.LocalType.IsValueType)
      return new AstReadLocalAddr(loc);
    return new AstReadLocalRef { LocalType = loc.LocalType, LocalIndex = loc.LocalIndex };
  }

  public static IAstRefOrAddr ReadPropertyRA(IAstRefOrAddr sourceObject, PropertyInfo propertyInfo)
  {
    if (propertyInfo.PropertyType.IsValueType)
      return new AstValueToAddr(
        new AstReadPropertyValue { SourceObject = sourceObject, PropertyInfo = propertyInfo });

    return new AstReadPropertyRef { SourceObject = sourceObject, PropertyInfo = propertyInfo };
  }

  public static IAstRefOrValue ReadPropertyRV(IAstRefOrAddr sourceObject, PropertyInfo propertyInfo)
  {
    if (propertyInfo.PropertyType.IsValueType)
      return new AstReadPropertyValue { SourceObject = sourceObject, PropertyInfo = propertyInfo };
    return new AstReadPropertyRef { SourceObject = sourceObject, PropertyInfo = propertyInfo };
  }

  public static IAstRefOrAddr ReadThis(Type thisType)
  {
    if (thisType.IsValueType)
      return new AstReadThisAddr { ThisType = thisType };
    return new AstReadThisRef { ThisType = thisType };
  }

  public static IAstRefOrValue ReadMembersChain(IAstRefOrAddr sourceObject, IEnumerable<MemberInfo> membersChain)
  {   
    var src = sourceObject;
    using var enumerator = membersChain.GetEnumerator();
    MemberInfo cur = null;
    if (enumerator.MoveNext())
    {
      cur = enumerator.Current;
    }
    while (enumerator.MoveNext())
    {
      src = ReadMemberRA(src, cur);
      cur = enumerator.Current;
    }
    return ReadMemberRV(src, cur);
  }
  public static IAstRefOrValue ReadMembersChain(IAstRefOrAddr sourceObject, MemberInfo membersChainOfOne)
  {     
    return ReadMemberRV(sourceObject, membersChainOfOne);
  }
  public static IAstStackItem ReadMember(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
  {
    if (memberInfo.MemberType == MemberTypes.Method)
    {
      var methodInfo = memberInfo.DeclaringType.GetMethod(memberInfo.Name);
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

  public static IAstRefOrAddr ReadMemberRA(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
  {
    if (memberInfo.MemberType == MemberTypes.Method)
    {
      var methodInfo = memberInfo.DeclaringType.GetMethod(memberInfo.Name);
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

  public static IAstRefOrValue ReadMemberRV(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
  {
    if (memberInfo.MemberType == MemberTypes.Method)
    {
      var methodInfo = memberInfo.DeclaringType.GetMethod(memberInfo.Name);
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

  public static IAstNode WriteMembersChain(
    IEnumerable<MemberInfo> membersChain1,
    IAstRefOrAddr targetObject,
    IAstRefOrValue value)
  {
    //todo: need to optimize
    var membersChain = membersChain1.ToArray();

    if (membersChain.Length == 1)
      return WriteMember(membersChain[0], targetObject, value);

    var readTarget = targetObject;

    for (var i = 0; i < membersChain.Length - 1; ++i)
      readTarget = ReadMemberRA(readTarget, membersChain[i]);
    return WriteMember(membersChain[membersChain.Length - 1], readTarget, value);
  }

  public static IAstNode WriteMember(MemberInfo memberInfo, IAstRefOrAddr targetObject, IAstRefOrValue value)
  {
    if (memberInfo.MemberType == MemberTypes.Field)
      return new AstWriteField { FieldInfo = (FieldInfo)memberInfo, TargetObject = targetObject, Value = value };
    return new AstWriteProperty(targetObject, value, (PropertyInfo)memberInfo);
  }
}