namespace EmitMapper.Conversion;

using System;
using System.Reflection;

using EmitMapper.Utils;

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
        return ChangeType(value, typeof(TFrom), typeof(TTo));
    }

    public static object ChangeType(object value, Type typeFrom, Type typeTo)
    {
        if (value == null)
            return null;

        if (typeTo.IsEnum)
            return ConvertToEnum(value, typeFrom, typeTo);

        if (typeFrom.IsEnum)
        {
            if (typeTo == typeof(string))
                return value.ToString();
            return ChangeType(
                Convert.ChangeType(value, Enum.GetUnderlyingType(typeFrom)),
                Enum.GetUnderlyingType(typeFrom),
                typeTo);
        }

        if (typeTo == typeof(Guid))
        {
            var r = new Guid(value.ToString()!);
            return r == Guid.Empty ? new Guid() : r;
        }

        var isFromNullable = ReflectionUtils.IsNullable(typeFrom);
        var isToNullable = ReflectionUtils.IsNullable(typeTo);

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
            return (TEnum)Enum.Parse(typeof(TEnum), str);
        }

        return (TEnum)Convert.ChangeType(obj, typeof(TUnder));
    }

    public static MethodInfo GetConversionMethod(Type from, Type to)
    {
        if (from == null || to == null)
            return null;

        if (to == typeof(string))
            return typeof(EMConvert).GetMethod("ObjectToString", BindingFlags.Static | BindingFlags.Public);

        if (to.IsEnum)
            return typeof(EMConvert).GetMethod("ToEnum", BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(to, Enum.GetUnderlyingType(to));

        if (IsComplexConvert(from) || IsComplexConvert(to))
            return typeof(EMConvert).GetMethod("ChangeTypeGeneric", BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(from, to);
        return null;
    }

    private static bool IsComplexConvert(Type type)
    {
        if (type.IsEnum)
            return true;
        if (ReflectionUtils.IsNullable(type))
            if (Nullable.GetUnderlyingType(type).IsEnum)
                return true;
        return false;
    }

    private static object ConvertToEnum(object value, Type typeFrom, Type typeTo)
    {
        if (!typeFrom.IsEnum)
            if (typeFrom == typeof(string))
                return Enum.Parse(typeTo, value.ToString());
        return Enum.ToObject(typeTo, Convert.ChangeType(value, Enum.GetUnderlyingType(typeTo)));
    }
}