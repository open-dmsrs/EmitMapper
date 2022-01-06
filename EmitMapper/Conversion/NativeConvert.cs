using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.Utils;

namespace EmitMapper.Conversion;

internal class NativeConverter
{
    private static readonly Type[] _ConvertTypes =
    {
        Meta<Boolean>.Type, Meta<Char>.Type, Meta<SByte>.Type, Meta<Byte>.Type, Meta<short>.Type, Meta<int>.Type, Meta<long>.Type,
        Meta<ushort>.Type, Meta<uint>.Type, Meta<ulong>.Type, Meta<float>.Type, Meta<Double>.Type, Meta<Decimal>.Type,
        Meta<DateTime>.Type, Meta<String>.Type
    };

    public static bool IsNativeConvertionPossible(Type from, Type to)
    {
        if (from == null || to == null)
            return false;

        if (_ConvertTypes.Contains(from) && _ConvertTypes.Contains(to))
            return true;

        if (to == Meta<String>.Type)
            return true;

        if (from == Meta<String>.Type && to == Meta<Guid>.Type)
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

    private static readonly MethodInfo ObjectToStringMethod = Meta<NativeConverter>.Type.GetMethod(nameof(ObjectToString), BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo[] ConvertMethods = TypeHome.Convert.GetMethods(BindingFlags.Static | BindingFlags.Public);

    private static readonly MethodInfo ChangeTypeMethod = Meta<EMConvert>.Type.GetMethod(nameof(EMConvert.ChangeType), new[] { Meta<object>.Type, Meta<Type>.Type, Meta<Type>.Type });

    public static IAstRefOrValue Convert(Type destinationType, Type sourceType, IAstRefOrValue sourceValue)
    {
        if (destinationType == sourceValue.ItemType)
            return sourceValue;

        if (destinationType == Meta<String>.Type)
            return new AstCallMethodRef(ObjectToStringMethod, null, new List<IAstStackItem> { sourceValue });

        foreach (var m in ConvertMethods)
            if (m.ReturnType == destinationType)
            {
                var parameters = m.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType == sourceType)
                    return AstBuildHelper.CallMethod(m, null, new List<IAstStackItem> { sourceValue });
            }

        return AstBuildHelper.CallMethod(ChangeTypeMethod, null, new List<IAstStackItem>
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