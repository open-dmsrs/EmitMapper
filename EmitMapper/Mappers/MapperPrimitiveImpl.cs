namespace EmitMapper.Mappers;

using System;
using System.Collections;

using EmitMapper.Conversion;
using EmitMapper.EmitInvoker.Methods;
using EmitMapper.MappingConfiguration;
using EmitMapper.Utils;

/// <summary>
///     Mapper for primitive objects
/// </summary>
internal class MapperPrimitiveImpl : CustomMapperImpl
{
    private new readonly MethodInvokerFunc_1 _converter;

    public MapperPrimitiveImpl(
        ObjectMapperManager mapperMannager,
        Type TypeFrom,
        Type TypeTo,
        IMappingConfigurator mappingConfigurator)
        : base(mapperMannager, TypeFrom, TypeTo, mappingConfigurator, null)
    {
        var to = TypeTo == typeof(IEnumerable) ? typeof(object) : TypeTo;
        var from = TypeFrom == typeof(IEnumerable) ? typeof(object) : TypeFrom;

        var staticConv = mappingConfigurator.GetStaticConvertersManager() ?? StaticConvertersManager.DefaultInstance;
        var converterMethod = staticConv.GetStaticConverter(from, to);

        if (converterMethod != null)
            this._converter = (MethodInvokerFunc_1)MethodInvoker.GetMethodInvoker(null, converterMethod);
    }

    /// <summary>
    ///     Copies object properties and members of "from" to object "to"
    /// </summary>
    /// <param name="from">Source object</param>
    /// <param name="to">Destination object</param>
    /// <returns>Destination object</returns>
    public override object MapImpl(object from, object to, object state)
    {
        if (this._converter == null)
            return from;
        return this._converter.CallFunc(from);
    }

    /// <summary>
    ///     Creates an instance of destination object
    /// </summary>
    /// <returns>Destination object</returns>
    public override object CreateTargetInstance()
    {
        return null;
    }

    internal static bool IsSupportedType(Type type)
    {
        return type.IsPrimitive || type == typeof(decimal) || type == typeof(float) || type == typeof(double)
               || type == typeof(long) || type == typeof(ulong) || type == typeof(short) || type == typeof(Guid)
               || type == typeof(string)
               || ReflectionUtils.IsNullable(type) && IsSupportedType(Nullable.GetUnderlyingType(type)) || type.IsEnum;
    }
}