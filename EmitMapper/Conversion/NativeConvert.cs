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
    Metadata<bool>.Type, Metadata<char>.Type, Metadata<sbyte>.Type, Metadata<byte>.Type, Metadata<short>.Type,
    Metadata<int>.Type, Metadata<long>.Type, Metadata<ushort>.Type, Metadata<uint>.Type, Metadata<ulong>.Type,
    Metadata<float>.Type, Metadata<double>.Type, Metadata<decimal>.Type, Metadata<DateTime>.Type, Metadata<string>.Type
  };

  private static readonly MethodInfo ObjectToStringMethod = Metadata<NativeConverter>.Type.GetMethod(
    nameof(ObjectToString),
    BindingFlags.NonPublic | BindingFlags.Static);

  private static readonly MethodInfo[] ConvertMethods =
    Metadata.Convert.GetMethods(BindingFlags.Static | BindingFlags.Public);

  private static readonly MethodInfo ChangeTypeMethod = Metadata<EMConvert>.Type.GetMethod(
    nameof(EMConvert.ChangeType),
    new[] { Metadata<object>.Type, Metadata<Type>.Type, Metadata<Type>.Type });

  private static readonly LazyConcurrentDictionary<TypesPair, bool> IsNativeConvertionPossibleCache =
    new(new TypesPair());

  public static bool IsNativeConvertionPossible(Type f, Type t)
  {
    return IsNativeConvertionPossibleCache.GetOrAdd(
      new TypesPair(f, t),
      p =>
      {
        var from = p.SourceType;
        var to = p.DestinationType;

        if (from == null || to == null)
          return false;

        if (_ConvertTypes.Contains(from) && _ConvertTypes.Contains(to))
          return true;

        if (to == Metadata<string>.Type)
          return true;

        if (from == Metadata<string>.Type && to == Metadata<Guid>.Type)
          return true;

        if (from.IsEnum && to.IsEnum)
          return true;

        if (from.IsEnum && _ConvertTypes.Contains(to))
          return true;

        if (to.IsEnum && _ConvertTypes.Contains(from))
          return true;

        if (ReflectionHelper.IsNullable(from))
          return IsNativeConvertionPossible(from.GetUnderlyingTypeCache(), to);

        if (ReflectionHelper.IsNullable(to))
          return IsNativeConvertionPossible(from, to.GetUnderlyingTypeCache());

        return false;
      });
  }

  public static IAstRefOrValue Convert(Type destinationType, Type sourceType, IAstRefOrValue sourceValue)
  {
    if (destinationType == sourceValue.ItemType)
      return sourceValue;

    if (destinationType == Metadata<string>.Type)
      return new AstCallMethodRef(ObjectToStringMethod, null, new List<IAstStackItem> { sourceValue });

    foreach (var m in ConvertMethods)
      if (m.ReturnType == destinationType)
      {
        var parameters = m.GetParameters();
        if (parameters.Length == 1 && parameters[0].ParameterType == sourceType)
          return AstBuildHelper.CallMethod(m, null, new List<IAstStackItem> { sourceValue });
      }

    return AstBuildHelper.CallMethod(
      ChangeTypeMethod,
      null,
      new List<IAstStackItem>
        { sourceValue, new AstTypeof { Type = sourceType }, new AstTypeof { Type = destinationType } }
    );
  }

  internal static string ObjectToString(object obj)
  {
    if (obj == null)
      return null;
    return obj.ToString();
  }
}