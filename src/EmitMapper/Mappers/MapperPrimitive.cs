namespace EmitMapper.Mappers;

/// <summary>
///   Mapper for primitive objects
/// </summary>
internal class MapperPrimitive : CustomMapper
{
	private static readonly LazyConcurrentDictionary<Type, bool> IsSupported = new();

	private readonly MethodInvokerFunc1 _converter;

	/// <summary>
	///   Initializes a new instance of the <see cref="MapperPrimitive" /> class.
	/// </summary>
	/// <param name="objectMapperManager">The object mapper manager.</param>
	/// <param name="typeFrom">The type from.</param>
	/// <param name="typeTo">The type to.</param>
	/// <param name="mappingConfigurator">The mapping configurator.</param>
	public MapperPrimitive(
	  Mapper? objectMapperManager,
	  Type typeFrom,
	  Type typeTo,
	  IMappingConfigurator? mappingConfigurator)
	  : base(objectMapperManager, typeFrom, typeTo, mappingConfigurator, null)
	{
		var to = typeTo == Metadata<IEnumerable>.Type ? Metadata<object>.Type : typeTo;
		var from = typeFrom == Metadata<IEnumerable>.Type ? Metadata<object>.Type : typeFrom;

		var staticConv = mappingConfigurator.GetStaticConvertersManager() ?? StaticConvertersManager.DefaultInstance;
		var converterMethod = staticConv.GetStaticConverter(from, to);

		if (converterMethod is not null)
		{
			_converter = (MethodInvokerFunc1)EmitInvoker.Methods.MethodInvoker.GetMethodInvoker(null, converterMethod);
		}
	}

	/// <summary>
	///   Creates an instance of destination object
	/// </summary>
	/// <returns>Destination object</returns>
	public override object? CreateTargetInstance()
	{
		return null;
	}

	/// <summary>
	///   Copies object properties and members of "from" to object "to"
	/// </summary>
	/// <param name="from">Source object</param>
	/// <param name="to">Destination object</param>
	/// <param name="state"></param>
	/// <returns>Destination object</returns>
	public override object? MapCore(object? from, object? to, object state)
	{
		if (_converter is null)
		{
			return from;
		}

		return _converter.CallFunc(from);
	}

	/// <summary>
	///   Are the supported type.
	/// </summary>
	/// <param name="t">The t.</param>
	/// <returns>A bool.</returns>
	internal static bool IsSupportedType(Type t)
	{
		return IsSupported.GetOrAdd(
		  t,
		  type => type.IsPrimitive || type == Metadata<decimal>.Type || type == Metadata<float>.Type
				  || type == Metadata<double>.Type || type == Metadata<long>.Type || type == Metadata<ulong>.Type
				  || type == Metadata<short>.Type || type == Metadata<Guid>.Type || type == Metadata<string>.Type
				  || ReflectionHelper.IsNullable(type) && IsSupportedType(type.GetUnderlyingTypeCache()) || type.IsEnum);
	}
}