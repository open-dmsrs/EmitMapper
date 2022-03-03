using EmitMapper.Mappers;

namespace EmitMapper;

public class MapperDescription
{
  public int Id;

  public MapperKey Key;

  public MapperBase Mapper;

  public MapperDescription(MapperBase mapper, MapperKey key, int id)
  {
    Mapper = mapper;
    Key = key;
    Id = id;
  }
}