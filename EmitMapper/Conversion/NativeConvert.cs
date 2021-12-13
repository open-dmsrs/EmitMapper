namespace EmitMapper.Conversion;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.Utils;

internal class NativeConverter
{
    private static readonly Type[] _ConvertTypes =
        {
            typeof(bool), typeof(char), typeof(sbyte), typeof(byte), typeof(short), typeof(int), typeof(long),
            typeof(ushort), typeof(uint), typeof(ulong), typeof(float), typeof(double), typeof(decimal),
            typeof(DateTime), typeof(string)
        };

    public static bool IsNativeConvertionPossible(Type from, Type to)
    {
        if (from == null || to == null)
            return false;

        if (_ConvertTypes.Contains(from) && _ConvertTypes.Contains(to))
            return true;

        if (to == typeof(string))
            return true;

        if (from == typeof(string) && to == typeof(Guid))
            return true;

        if (from.IsEnum && to.IsEnum)
            return true;

        if (from.IsEnum && _ConvertTypes.Contains(to))
            return true;

        if (to.IsEnum && _ConvertTypes.Contains(from))
            return true;

        if (ReflectionUtils.IsNullable(from))
            return IsNativeConvertionPossible(Nullable.GetUnderlyingType(from), to);

        if (ReflectionUtils.IsNullable(to))
            return IsNativeConvertionPossible(from, Nullable.GetUnderlyingType(to));

        return false;
    }

    public static IAstRefOrValue Convert(Type destinationType, Type sourceType, IAstRefOrValue sourceValue)
    {
        if (destinationType == sourceValue.ItemType)
            return sourceValue;

        if (destinationType == typeof(string))
            return new AstCallMethodRef(
                typeof(NativeConverter).GetMethod(nameof(ObjectToString), BindingFlags.NonPublic | BindingFlags.Static),
                null,
                new List<IAstStackItem> { sourceValue });

        foreach (var m in typeof(Convert).GetMethods(BindingFlags.Static | BindingFlags.Public))
            if (m.ReturnType == destinationType)
            {
                var parameters = m.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType == sourceType)
                    return AstBuildHelper.CallMethod(m, null, new List<IAstStackItem> { sourceValue });
            }

        return AstBuildHelper.CallMethod(
            typeof(EMConvert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type), typeof(Type) }),
            null,
            new List<IAstStackItem>
                {
                    sourceValue, new AstTypeof { Type = sourceType }, new AstTypeof { Type = destinationType }
                });
    }

    internal static string ObjectToString(object obj)
    {
        if (obj == null)
            return null;
        return obj.ToString();
    }
}