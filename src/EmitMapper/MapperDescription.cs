using EmitMapper.Mappers;

namespace EmitMapper;
/// <summary>
/// The mapper description.
/// </summary>

public class MapperDescription
{
  public int Id;

  public MapperKey Key;

  public MapperBase Mapper;

  /// <summary>
  /// Initializes a new instance of the <see cref="MapperDescription"/> class.
  /// </summary>
  /// <param name="mapper">The mapper.</param>
  /// <param name="key">The key.</param>
  /// <param name="id">The id.</param>
  public MapperDescription(MapperBase mapper, MapperKey key, int id)
  {
    Mapper = mapper;
    Key = key;
    Id = id;
  }
}