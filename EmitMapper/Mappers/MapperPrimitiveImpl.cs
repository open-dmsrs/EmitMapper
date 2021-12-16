using System;
using System.Collections;
using EmitMapper.Conversion;
using EmitMapper.EmitInvoker.Methods;
using EmitMapper.MappingConfiguration;
using EmitMapper.Utils;

namespace EmitMapper.Mappers;

/// <summary>
///     Mapper for primitive objects
/// </summary>
internal class MapperPrimitiveImpl : CustomMapperImpl
{
    private readonly MethodInvokerFunc1 _converter;

    public MapperPrimitiveImpl(
        ObjectMapperManager objectMapperManager,
        Type typeFrom,
        Type typeTo,
        IMappingConfigurator mappingConfigurator)
        : base(objectMapperManager, typeFrom, typeTo, mappingConfigurator, null)
    {
        var to = typeTo == typeof(IEnumerable) ? typeof(object) : typeTo;
        var from = typeFrom == typeof(IEnumerable) ? typeof(object) : typeFrom;

        var staticConv = mappingConfigurator.GetStaticConvertersManager() ?? StaticConvertersManager.DefaultInstance;
        var converterMethod = staticConv.GetStaticConverter(from, to);

        if (converterMethod != null)
            _converter = (MethodInvokerFunc1)MethodInvoker.GetMethodInvoker(null, converterMethod);
    }

    internal static bool IsSupportedType(Type type)
    {
        return type.IsPrimitive || type == typeof(decimal) || type == typeof(float) || type == typeof(double)
               || type == typeof(long) || type == typeof(ulong) || type == typeof(short) || type == typeof(Guid)
               || type == typeof(string)
               || ReflectionUtils.IsNullable(type) && IsSupportedType(Nullable.GetUnderlyingType(type)) || type.IsEnum;
    }

    /// <summary>
    ///     Copies object properties and members of "from" to object "to"
    /// </summary>
    /// <param name="from">Source object</param>
    /// <param name="to">Destination object</param>
    /// <returns>Destination object</returns>
    public override object MapImpl(object from, object to, object state)
    {
        if (_converter == null)
            return from;
        return _converter.CallFunc(from);
    }

    /// <summary>
    ///     Creates an instance of destination object
    /// </summary>
    /// <returns>Destination object</returns>
    public override object CreateTargetInstance()
    {
        return null;
    }
}