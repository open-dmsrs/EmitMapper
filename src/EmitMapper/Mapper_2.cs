namespace EmitMapper;

/// <summary>
///   The mapper.
/// </summary>
/// <typeparam name="TFrom"></typeparam>
/// <typeparam name="TTo"></typeparam>
public class Mapper<TFrom, TTo>
{
	private readonly MapperBase _mapperImpl;

	/// <summary>
	///   Initializes a new instance of the <see cref="Mapper" /> class.
	/// </summary>
	/// <param name="mapperImpl">The mapper impl.</param>
	public Mapper(MapperBase mapperImpl)
	{
		_mapperImpl = mapperImpl;
	}

	/// <summary>
	///   Maps the
	///   <see>
	///     <cref>TTo</cref>
	///   </see>
	///   .
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="to">The to.</param>
	/// <param name="state">The state.</param>
	/// <returns>A TTo.</returns>
	public TTo Map(TFrom from, TTo to, object state)
	{
		return (TTo)_mapperImpl.Map(from, to, state);
	}

	/// <summary>
	///   Maps the
	///   <see>
	///     <cref>TTo</cref>
	///   </see>
	///   .
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="to">The to.</param>
	/// <returns>A TTo.</returns>
	public TTo Map(TFrom from, TTo to)
	{
		return (TTo)_mapperImpl.Map(from, to, null);
	}

	/// <summary>
	///   Maps the
	///   <see>
	///     <cref>TTo</cref>
	///   </see>
	///   .
	/// </summary>
	/// <param name="from">The from.</param>
	/// <returns>A TTo.</returns>
	public TTo Map(TFrom from)
	{
		return (TTo)_mapperImpl.Map(from);
	}

	/// <summary>
	///   Maps the enum.
	/// </summary>
	/// <param name="sourceCollection">The source collection.</param>
	/// <returns><![CDATA[IEnumerable<TTo>]]></returns>
	public IEnumerable<TTo> MapEnum(IEnumerable<TFrom> sourceCollection)
	{
		foreach (var src in sourceCollection)
		{
			yield return Map(src);
		}
	}

	/// <summary>
	///   Maps the enum.
	/// </summary>
	/// <param name="sourceCollection">The source collection.</param>
	/// <returns><![CDATA[List<TTo>]]></returns>
	public List<TTo> MapEnum(List<TFrom> sourceCollection)
	{
		var result = new List<TTo>(sourceCollection.Count);

		foreach (var src in sourceCollection)
		{
			result.Add(Map(src));
		}

		return result;
	}

	/// <summary>
	///   Maps the using state.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="state">The state.</param>
	/// <returns>A TTo.</returns>
	public TTo MapUsingState(TFrom from, object state)
	{
		return (TTo)_mapperImpl.Map(from, null, state);
	}
}