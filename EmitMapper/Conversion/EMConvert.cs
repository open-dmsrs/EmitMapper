using System;
using System.Reflection;
using EmitMapper.Utils;

namespace EmitMapper.Conversion;

public class EMConvert
{
  public static object ChangeType(object value, Type conversionType)
  {
    if (value == null)
      return null;
    return ChangeType(value, value.GetType(), conversionType);
  }

  public static object ChangeTypeGeneric<TFrom, TTo>(object value)
  {
    return ChangeType(value, Metadata<TFrom>.Type, Metadata<TTo>.Type);
  }

  public static object ChangeType(object value, Type typeFrom, Type typeTo)
  {
    if (value == null)
      return null;

    if (typeTo.IsEnum)
      return ConvertToEnum(value, typeFrom, typeTo);

    if (typeFrom.IsEnum)
    {
      if (typeTo == Metadata<string>.Type)
        return value.ToString();
      return ChangeType(
        Convert.ChangeType(value, Enum.GetUnderlyingType(typeFrom)),
        Enum.GetUnderlyingType(typeFrom),
        typeTo);
    }

    if (typeTo == Metadata<Guid>.Type)
    {
      var r = new Guid(value.ToString()!);
      return r == Guid.Empty ? new Guid() : r;
    }

    var isFromNullable = ReflectionHelper.IsNullable(typeFrom);
    var isToNullable = ReflectionHelper.IsNullable(typeTo);

    if (isFromNullable && !isToNullable)
      return ChangeType(value, Nullable.GetUnderlyingType(typeFrom), typeTo);

    if (isToNullable)
    {
      var ut = Nullable.GetUnderlyingType(typeTo);
      if (ut.IsEnum)
        return ConvertToEnum(value, typeFrom, ut);
      return ChangeType(value, typeFrom, ut);
    }

    return Convert.ChangeType(value, typeTo);
  }

  public static string ObjectToString(object obj)
  {
    if (obj == null)
      return null;
    return obj.ToString();
  }

  public static Guid StringToGuid(string str)
  {
    if (string.IsNullOrEmpty(str))
      return Guid.Empty;
    return new Guid(str);
  }

  public static TEnum ToEnum<TEnum, TUnder>(object obj)
  {
    if (obj is string)
    {
      var str = obj.ToString();
      return (TEnum)Enum.Parse(Metadata<TEnum>.Type, str);
    }

    return (TEnum)Convert.ChangeType(obj, Metadata<TUnder>.Type);
  }

  public static MethodInfo GetConversionMethod(Type from, Type to)
  {
    if (from == null || to == null)
      return null;

    if (to == Metadata<string>.Type)
      return Metadata<EMConvert>.Type.GetMethod(nameof(ObjectToString), BindingFlags.Static | BindingFlags.Public);

    if (to.IsEnum)
      return Metadata<EMConvert>.Type.GetMethod(nameof(ToEnum), BindingFlags.Static | BindingFlags.Public)
        ?.MakeGenericMethod(to, Enum.GetUnderlyingType(to));

    if (IsComplexConvert(from) || IsComplexConvert(to))
      return Metadata<EMConvert>.Type.GetMethod(nameof(ChangeTypeGeneric), BindingFlags.Static | BindingFlags.Public)
        ?.MakeGenericMethod(from, to);

    return null;
  }

  private static bool IsComplexConvert(Type type)
  {
    if (type.IsEnum)
      return true;
    if (ReflectionHelper.IsNullable(type))
      if (Nullable.GetUnderlyingType(type).IsEnum)
        return true;

    return false;
  }

  private static object ConvertToEnum(object value, Type typeFrom, Type typeTo)
  {
    if (!typeFrom.IsEnum)
      if (typeFrom == Metadata<string>.Type)
        return Enum.Parse(typeTo, value.ToString());

    return Enum.ToObject(typeTo, Convert.ChangeType(value, Enum.GetUnderlyingType(typeTo)));
  }
}