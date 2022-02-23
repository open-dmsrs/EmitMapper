using System;
using System.Collections.Generic;
using System.Threading;
using EmitMapper.EmitBuilders;
using EmitMapper.Mappers;
using EmitMapper.MappingConfiguration;
using EmitMapper.Utils;

namespace EmitMapper;

/// <summary>
///   Class for maintaining and generating Mappers.
/// </summary>
public class Mapper
{
  private static readonly Lazy<Mapper> LazyDefaultInstance = new();

  private static readonly Dictionary<MapperKey, MapperDescription> ObjectsMapperIds =
    new(new MapperKey(null, null, null, 0));

  private static int _totalInstCount;

  private readonly int _currentInstanceId;

  public Mapper()
  {
    _currentInstanceId = Interlocked.Increment(ref _totalInstCount);
  }

  public static Mapper Default => LazyDefaultInstance.Value;

  /// <summary>
  ///   Returns a Mapper instance for specified types.
  /// </summary>
  /// <typeparam name="TFrom">Type of source object</typeparam>
  /// <typeparam name="TTo">Type of destination object</typeparam>
  /// <returns></returns>
  public Mapper<TFrom, TTo> GetMapper<TFrom, TTo>()
  {
    return new Mapper<TFrom, TTo>(
      GetMapperImpl(Metadata<TFrom>.Type, Metadata<TTo>.Type, DefaultMapConfig.Instance));
  }

  /// <summary>
  ///   Returns a Mapper instance for specified types.
  /// </summary>
  /// <typeparam name="TFrom">Type of source object</typeparam>
  /// <typeparam name="TTo">Type of destination object</typeparam>
  /// <param name="mappingConfigurator">Object which configures mapping.</param>
  /// <returns>Mapper</returns>
  public Mapper<TFrom, TTo> GetMapper<TFrom, TTo>(IMappingConfigurator mappingConfigurator)
  {
    return new Mapper<TFrom, TTo>(GetMapperImpl(Metadata<TFrom>.Type, Metadata<TTo>.Type, mappingConfigurator));
  }

  /// <summary>
  ///   Returns a mapper implementation instance for specified types.
  /// </summary>
  /// <param name="from">Type of source object</param>
  /// <param name="to">Type of destination object</param>
  /// <param name="mappingConfigurator">Object which configures mapping.</param>
  /// <returns>Mapper</returns>
  public MapperBase GetMapperImpl(Type from, Type to, IMappingConfigurator mappingConfigurator)
  {
    return GetMapperInt(from, to, mappingConfigurator).Mapper;
  }

  internal MapperDescription GetMapperInt(Type from, Type to, IMappingConfigurator mappingConfigurator)
  {
    to ??= Metadata<object>.Type;
    from ??= Metadata<object>.Type;
    var mapperTypeKey = new MapperKey(from, to, mappingConfigurator, _currentInstanceId);

    if (ObjectsMapperIds.TryGetValue(mapperTypeKey, out var result))
      return result;

    lock (ObjectsMapperIds)
    {
      if (ObjectsMapperIds.TryGetValue(mapperTypeKey, out result))
        return result;

      result = new MapperDescription(null, mapperTypeKey, 0);
      ObjectsMapperIds.Add(mapperTypeKey, result);

      var mapperTypeName = mapperTypeKey.GetMapperTypeName();
      MapperBase createdMapper;

      if (MapperPrimitive.IsSupportedType(to))
      {
        createdMapper = new MapperPrimitive(this, from, to, mappingConfigurator);
      }
      else if (MapperForCollectionImpl.IsSupportedType(to))
      {
        var mapperDescr = GetMapperInt(
          MapperForCollectionImpl.GetSubMapperTypeFrom(from),
          MapperForCollectionImpl.GetSubMapperTypeTo(to),
          mappingConfigurator);

        createdMapper = MapperForCollectionImpl.CreateInstance(
          mapperTypeName,
          this,
          from,
          to,
          mapperDescr,
          mappingConfigurator);
      }
      else
      {
        createdMapper = BuildObjectsMapper(mapperTypeName, from, to, mappingConfigurator);
      }

      result.Mapper = createdMapper;

      return result;
    }
  }

  private MapperBase BuildObjectsMapper(
    string mapperTypeName,
    Type from,
    Type to,
    IMappingConfigurator mappingConfigurator)
  {
    var typeBuilder = DynamicAssemblyManager.DefineMapperType(mapperTypeName);
    CreateTargetInstanceBuilder.BuildCreateTargetInstanceMethod(to, typeBuilder);

    var mappingBuilder = new MappingBuilder(this, from, to, typeBuilder, mappingConfigurator);
    mappingBuilder.BuildCopyImplMethod();

    var result = ObjectFactory.CreateInstance<MapperBase>(typeBuilder.CreateType());
    result.Initialize(this, from, to, mappingConfigurator, mappingBuilder.StoredObjects.ToArray());

    return result;
  }
}