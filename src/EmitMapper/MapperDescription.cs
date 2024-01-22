namespace EmitMapper;

/// <summary>
///   The mapper description.
/// </summary>
public class MapperDescription
{
	/// <summary>
	/// Id
	/// </summary>
	/// <value cref="int">int</value>
	public int Id
	{
		get;
	}

	/// <summary>
	/// Key
	/// </summary>
	/// <value cref="MapperKey">MapperKey</value>
	public MapperKey Key
	{
		get;
	}

	/// <summary>
	/// Mapper
	/// </summary>
	public MapperBase? Mapper;

	/// <summary>
	///   Initializes a new instance of the <see cref="MapperDescription" /> class.
	/// </summary>
	/// <param name="mapper">The mapper.</param>
	/// <param name="key">The key.</param>
	/// <param name="id">The id.</param>
	public MapperDescription(MapperBase? mapper, MapperKey key, int id)
	{
		Mapper = mapper;
		Key = key;
		Id = id;
	}
}