namespace EmitMapper.Mappers;

/// <summary>
///   Mapper for collections. It can copy Array, List&lt;&gt;, ArrayList collections.
///   Collection type in source object and destination object can differ.
/// </summary>
public class MapperForCollection : CustomMapper
{
	private static readonly MethodInfo? CopyToListMethod = Metadata<MapperForCollection>.Type.GetMethod(nameof(CopyToList), BindingFlags.Instance | BindingFlags.NonPublic);

	private static readonly MethodInfo? CopyToListScalarMethod = Metadata<MapperForCollection>.Type.GetMethod(nameof(CopyToListScalar), BindingFlags.Instance | BindingFlags.NonPublic);

	private static readonly LazyConcurrentDictionary<Type, bool> IsSupportedCache = new();

	private MapperDescription subMapper;

	/// <summary>
	///   Initializes a new instance of the <see cref="MapperForCollection" /> class.
	/// </summary>
	protected MapperForCollection()
	  : base(null, null, null, null, null)
	{
	}

	/// <summary>
	///   Creates an instance of Mapper for collections.
	/// </summary>
	/// <param name="mapperName">Mapper name. It is used for registration in Mappers repositories.</param>
	/// <param name="objectMapperManager">Mappers manager</param>
	/// <param name="typeFrom">Source type</param>
	/// <param name="typeTo">Destination type</param>
	/// <param name="subMapper"></param>
	/// <param name="mappingConfigurator"></param>
	/// <returns></returns>
	public static MapperForCollection CreateInstance(
	  string mapperName,
	  Mapper? objectMapperManager,
	  Type typeFrom,
	  Type typeTo,
	  MapperDescription subMapper,
	  IMappingConfigurator? mappingConfigurator)
	{
		var tb = DynamicAssemblyManager.DefineType("GenericListInv_" + mapperName, Metadata<MapperForCollection>.Type);

		switch (typeTo.IsGenericType)
		{
			case true when typeTo.GetGenericTypeDefinitionCache() == Metadata.List1:
			{
				var methodBuilder = tb.DefineMethod(
					nameof(CopyToListInvoke),
					MethodAttributes.Family | MethodAttributes.Virtual,
					Metadata<object>.Type,
					new[] { Metadata<IEnumerable>.Type });

				InvokeCopyImpl(typeTo, CopyToListMethod).Compile(new CompilationContext(methodBuilder.GetILGenerator()));

				methodBuilder = tb.DefineMethod(
					nameof(CopyToListScalarInvoke),
					MethodAttributes.Family | MethodAttributes.Virtual,
					Metadata<object>.Type,
					new[] { Metadata<object>.Type });

				InvokeCopyImpl(typeTo, CopyToListScalarMethod).Compile(new CompilationContext(methodBuilder.GetILGenerator()));

				break;
			}
		}

		var result = ObjectFactory.CreateInstance<MapperForCollection>(tb.CreateType());
		result.Initialize(objectMapperManager, typeFrom, typeTo, mappingConfigurator, null);
		result.subMapper = subMapper;

		return result;
	}

	/// <summary>
	///   Creates the target instance.
	/// </summary>
	/// <returns>An object.</returns>
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
	public override object? Map(object? from, object? to, object state)
	{
		return base.Map(from, null, state);
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
		switch (to)
		{
			case null when TargetConstructor is not null:
				to = TargetConstructor.CallFunc();

				break;
		}

		switch (TypeTo.IsArray)
		{
			case true when from is IEnumerable fromEnumerable:
				return CopyToArray(fromEnumerable);
			case true:
				return CopyScalarToArray(from);
		}

		switch (TypeTo.IsGenericType)
		{
			case true when TypeTo.GetGenericTypeDefinitionCache() == Metadata.List1:
			{
				switch (from)
				{
					case IEnumerable fromEnumerable:
						return CopyToListInvoke(fromEnumerable);
					default:
						return CopyToListScalarInvoke(from);
				}
			}
		}

		if (TypeTo == Metadata<ArrayList>.Type)
		{
			switch (from)
			{
				case IEnumerable fromEnumerable:
					return CopyToArrayList(fromEnumerable);
				default:
					return CopyToArrayListScalar(from);
			}
		}

		if (Metadata<IList>.Type.IsAssignableFrom(TypeTo))
		{
			return CopyToIList((IList)to, from);
		}

		return null;
	}

	/// <summary>
	///   Gets the sub mapper type from.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <returns>A Type.</returns>
	internal static Type GetSubMapperTypeFrom(Type from)
	{
		var result = ExtractElementType(from);

		switch (result)
		{
			case null:
				return from;
			default:
				return result;
		}
	}

	/// <summary>
	///   Gets the sub mapper type to.
	/// </summary>
	/// <param name="to">The to.</param>
	/// <returns>A Type.</returns>
	internal static Type GetSubMapperTypeTo(Type to)
	{
		return ExtractElementType(to);
	}

	/// <summary>
	///   Returns true if specified type is supported by this Mapper
	/// </summary>
	/// <param name="t"></param>
	/// <returns></returns>
	internal static bool IsSupportedType(Type t)
	{
		return IsSupportedCache.GetOrAdd(
		  t,
		  type => type.IsArray || type.IsGenericType && type.GetGenericTypeDefinitionCache() == Metadata.List1
							   || type == Metadata<ArrayList>.Type || Metadata<IList>.Type.IsAssignableFrom(type)
							   || Metadata.IList1.IsAssignableFrom(type));
	}

	/// <summary>
	///   Copies the to list.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="from">The from.</param>
	/// <returns><![CDATA[List<T>]]></returns>
	protected List<T> CopyToList<T>(IEnumerable from)
	{
		List<T> result;

		switch (from)
		{
			case ICollection collection:
				result = new List<T>(collection.Count);

				break;
			default:
				result = new List<T>();

				break;
		}

		foreach (var obj in from)
		{
			result.Add((T)subMapper.Mapper.Map(obj));
		}

		return result;
	}

	/// <summary>
	///   Copies the to list invoke.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <returns>An object.</returns>
	protected virtual object? CopyToListInvoke(IEnumerable from)
	{
		return null;
	}

	/// <summary>
	///   Copies the to list scalar.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="from">The from.</param>
	/// <returns><![CDATA[List<T>]]></returns>
	protected List<T> CopyToListScalar<T>(object from)
	{
		var result = new List<T>(1) { (T)subMapper.Mapper.Map(from) };

		return result;
	}

	/// <summary>
	///   Copies the to list scalar invoke.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <returns>An object.</returns>
	protected virtual object? CopyToListScalarInvoke(object? from)
	{
		return null;
	}

	/// <summary>
	///   Extracts the element type.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <returns>A Type.</returns>
	private static Type? ExtractElementType(Type collection)
	{
		switch (collection.IsArray)
		{
			case true:
				return collection.GetElementType();
		}

		if (collection == Metadata<ArrayList>.Type)
		{
			return Metadata<object>.Type;
		}

		switch (collection.IsGenericType)
		{
			case true when collection.GetGenericTypeDefinitionCache() == Metadata.List1:
				return collection.GetGenericArguments()[0];
			default:
				return null;
		}
	}

	/// <summary>
	///   Invokes the copy impl.
	/// </summary>
	/// <param name="copiedObjectType">The copied object type.</param>
	/// <param name="copyMethod">The copy method.</param>
	/// <returns>An IAstNode.</returns>
	private static IAstNode InvokeCopyImpl(Type copiedObjectType, MethodInfo? copyMethod)
	{
		var mi = copyMethod?.MakeGenericMethod(ExtractElementType(copiedObjectType));

		return new AstReturn
		{
			ReturnType = Metadata<object>.Type,
			ReturnValue = AstBuildHelper.CallMethod(
			mi,
			AstBuildHelper.ReadThis(Metadata<MapperForCollection>.Type),
			new List<IAstStackItem> { new AstReadArgumentRef { ArgumentIndex = 1, ArgumentType = Metadata<object>.Type } })
		};
	}

	/// <summary>
	///   Copies the scalar to array.
	/// </summary>
	/// <param name="scalar">The scalar.</param>
	/// <returns>An Array.</returns>
	private Array? CopyScalarToArray(object? scalar)
	{
		var result = Array.CreateInstance(TypeTo.GetElementType(), 1);
		result.SetValue(subMapper.Mapper.Map(scalar), 0);

		return result;
	}

	/// <summary>
	///   Copies the to array.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <returns>An Array.</returns>
	private Array? CopyToArray(IEnumerable from)
	{
		switch (from)
		{
			case ICollection collection:
			{
				var result = Array.CreateInstance(TypeTo.GetElementType(), collection.Count);
				var idx = 0;

				foreach (var obj in collection)
				{
					result.SetValue(subMapper.Mapper.Map(obj), idx++);
				}

				return result;
			}
			default:
			{
				var result = new ArrayList();

				foreach (var obj in from)
				{
					result.Add(obj);
				}

				return result.ToArray(TypeTo.GetElementType());
			}
		}
	}

	/// <summary>
	///   Copies the to array list.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <returns>An ArrayList.</returns>
	private ArrayList? CopyToArrayList(IEnumerable from)
	{
		switch (ShallowCopy)
		{
			case true when from is ICollection collection:
				return new ArrayList(collection);
			case true:
			{
				var res = new ArrayList();

				foreach (var obj in from)
				{
					res.Add(obj);
				}

				return res;
			}
		}

		ArrayList? result;

		switch (from)
		{
			case ICollection coll:
				result = new ArrayList(coll.Count);

				break;
			default:
				result = new ArrayList();

				break;
		}

		foreach (var obj in from)
		{
			switch (obj)
			{
				case null:
					result.Add(null);

					break;
				default:
				{
					var mapper = Mapper.GetMapper(obj.GetType(), obj.GetType(), MappingConfigurator);
					result.Add(mapper.Map(obj));

					break;
				}
			}
		}

		return result;
	}

	/// <summary>
	///   Copies the to array list scalar.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <returns>An ArrayList.</returns>
	private ArrayList? CopyToArrayListScalar(object? from)
	{
		var result = new ArrayList(1);

		switch (ShallowCopy)
		{
			case true:
				result.Add(from);

				return result;
		}

		var mapper = Mapper.GetMapper(from.GetType(), from.GetType(), MappingConfigurator);
		result.Add(mapper.Map(from));

		return result;
	}

	/// <summary>
	///   Copies the to i list.
	/// </summary>
	/// <param name="iList">The i list.</param>
	/// <param name="from">The from.</param>
	/// <returns>An object.</returns>
	private object? CopyToIList(IList? iList, object? from)
	{
		iList ??= ObjectFactory.CreateInstance<IList>(TypeTo);

		foreach (var obj in from is IEnumerable fromEnumerable ? fromEnumerable : from.AsEnumerable())
		{
			switch (obj)
			{
				case null:
					iList.Add(null);

					break;
				default:
				{
					if (RootOperation?.ShallowCopy != false)
					{
						iList.Add(obj);
					}
					else
					{
						var mapper = Mapper.GetMapper(obj.GetType(), obj.GetType(), MappingConfigurator);
						iList.Add(mapper.Map(obj));
					}

					break;
				}
			}
		}

		return iList;
	}
}