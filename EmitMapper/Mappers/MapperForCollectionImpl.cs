using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.MappingConfiguration;

namespace EmitMapper.Mappers;

/// <summary>
///   Mapper for collections. It can copy Array, List&lt;&gt;, ArrayList collections.
///   Collection type in source object and destination object can differ.
/// </summary>
public class MapperForCollectionImpl : CustomMapperImpl
{
  public static readonly Type MapperForCollectionImplType = Meta<MapperForCollectionImpl>.Type;
  private ObjectsMapperDescr _subMapper;

  protected MapperForCollectionImpl()
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
  public static MapperForCollectionImpl CreateInstance(
    string mapperName,
    ObjectMapperManager objectMapperManager,
    Type typeFrom,
    Type typeTo,
    ObjectsMapperDescr subMapper,
    IMappingConfigurator mappingConfigurator)
  {
    var tb = DynamicAssemblyManager.DefineType("GenericListInv_" + mapperName, MapperForCollectionImplType);

    if (typeTo.IsGenericType && typeTo.GetGenericTypeDefinition() == TypeHome.List1)
    {
      var methodBuilder = tb.DefineMethod(
        nameof(CopyToListInvoke),
        MethodAttributes.Family | MethodAttributes.Virtual,
        Meta<object>.Type,
        new[] { Meta<IEnumerable>.Type }
      );

      InvokeCopyImpl(typeTo, nameof(CopyToList)).Compile(new CompilationContext(methodBuilder.GetILGenerator()));

      methodBuilder = tb.DefineMethod(
        nameof(CopyToListScalarInvoke),
        MethodAttributes.Family | MethodAttributes.Virtual,
        Meta<object>.Type,
        new[] { Meta<object>.Type }
      );

      InvokeCopyImpl(typeTo, nameof(CopyToListScalar))
        .Compile(new CompilationContext(methodBuilder.GetILGenerator()));
    }

    var result = Expression.Lambda<Func<MapperForCollectionImpl>>(Expression.New(tb.CreateType()))
      .Compile()
      .Invoke();
    result.Initialize(objectMapperManager, typeFrom, typeTo, mappingConfigurator, null);
    result._subMapper = subMapper;

    return result;
  }

  /// <summary>
  ///   Returns true if specified type is supported by this Mapper
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  internal static bool IsSupportedType(Type type)
  {
    return type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) ||
           type == Meta<ArrayList>.Type || Meta<IList>.Type.IsAssignableFrom(type) ||
           typeof(IList<>).IsAssignableFrom(type);
  }

  internal static Type GetSubMapperTypeTo(Type to)
  {
    return ExtractElementType(to);
  }

  internal static Type GetSubMapperTypeFrom(Type from)
  {
    var result = ExtractElementType(from);
    if (result == null)
      return from;

    return result;
  }

  private static IAstNode InvokeCopyImpl(Type copiedObjectType, string copyMethodName)
  {
    var mi = MapperForCollectionImplType
      .GetMethod(copyMethodName, BindingFlags.Instance | BindingFlags.NonPublic) // fixed BUG 
      ?.MakeGenericMethod(ExtractElementType(copiedObjectType));

    return new AstReturn
    {
      ReturnType = Meta<object>.Type,
      ReturnValue = AstBuildHelper.CallMethod(
        mi,
        AstBuildHelper.ReadThis(MapperForCollectionImplType),
        new List<IAstStackItem> { new AstReadArgumentRef { ArgumentIndex = 1, ArgumentType = Meta<object>.Type } }
      )
    };
  }

  private static Type ExtractElementType(Type collection)
  {
    if (collection.IsArray)
      return collection.GetElementType();
    if (collection == Meta<ArrayList>.Type)
      return Meta<object>.Type;
    if (collection.IsGenericType && collection.GetGenericTypeDefinition() == TypeHome.List1)
      return collection.GetGenericArguments()[0];
    return null;
  }

  /// <summary>
  ///   Copies object properties and members of "from" to object "to"
  /// </summary>
  /// <param name="from">Source object</param>
  /// <param name="to">Destination object</param>
  /// <param name="state"></param>
  /// <returns>Destination object</returns>
  public override object MapImpl(object from, object to, object state)
  {
    if (to == null && TargetConstructor != null)
      to = TargetConstructor.CallFunc();

    if (TypeTo.IsArray)
    {
      if (from is IEnumerable fromEnumerable)
        return CopyToArray(fromEnumerable);
      return CopyScalarToArray(from);
    }

    if (TypeTo.IsGenericType && TypeTo.GetGenericTypeDefinition() == TypeHome.List1)
    {
      if (from is IEnumerable fromEnumerable)
        return CopyToListInvoke(fromEnumerable);
      return CopyToListScalarInvoke(from);
    }

    if (TypeTo == Meta<ArrayList>.Type)
    {
      if (from is IEnumerable fromEnumerable)
        return CopyToArrayList(fromEnumerable);
      return CopyToArrayListScalar(from);
    }

    if (Meta<IList>.Type.IsAssignableFrom(TypeTo))
      return CopyToIList((IList)to, from);
    return null;
  }

  /// <summary>
  ///   Copies object properties and members of "from" to object "to"
  /// </summary>
  /// <param name="from">Source object</param>
  /// <param name="to">Destination object</param>
  /// <param name="state"></param>
  /// <returns>Destination object</returns>
  public override object Map(object from, object to, object state)
  {
    return base.Map(from, null, state);
  }

  public override object CreateTargetInstance()
  {
    return null;
  }

  protected virtual object CopyToListInvoke(IEnumerable from)
  {
    return null;
  }

  protected virtual object CopyToListScalarInvoke(object from)
  {
    return null;
  }

  protected List<T> CopyToList<T>(IEnumerable from)
  {
    List<T> result;
    if (from is ICollection collection)
      result = new List<T>(collection.Count);
    else
      result = new List<T>();
    foreach (var obj in from)
      result.Add((T)_subMapper.Mapper.Map(obj));
    return result;
  }

  protected List<T> CopyToListScalar<T>(object from)
  {
    var result = new List<T>(1) { (T)_subMapper.Mapper.Map(from) };
    return result;
  }

  private object CopyToIList(IList iList, object from)
  {
    iList ??= Expression.Lambda<Func<IList>>(Expression.New(TypeTo)).Compile()();
    foreach (var obj in from is IEnumerable fromEnumerable ? fromEnumerable : new[] { from })
      if (obj == null)
      {
        iList.Add(null);
      }
      else if (RootOperation == null || RootOperation.ShallowCopy)
      {
        iList.Add(obj);
      }
      else
      {
        var mapper = ObjectMapperManager.GetMapperImpl(
          obj.GetType(),
          obj.GetType(),
          MappingConfigurator
        );
        iList.Add(mapper.Map(obj));
      }

    return iList;
  }

  private Array CopyToArray(IEnumerable from)
  {
    if (from is ICollection collection)
    {
      var result = Array.CreateInstance(TypeTo.GetElementType(), collection.Count);
      var idx = 0;
      foreach (var obj in collection)
        result.SetValue(_subMapper.Mapper.Map(obj), idx++);
      return result;
    }
    else
    {
      var result = new ArrayList();
      foreach (var obj in from)
        result.Add(obj);
      return result.ToArray(TypeTo.GetElementType());
    }
  }

  private ArrayList CopyToArrayList(IEnumerable from)
  {
    if (ShallowCopy)
    {
      if (from is ICollection collection)
        return new ArrayList(collection);

      var res = new ArrayList();
      foreach (var obj in from)
        res.Add(obj);
      return res;
    }

    ArrayList result;
    if (from is ICollection coll)
      result = new ArrayList(coll.Count);
    else
      result = new ArrayList();

    foreach (var obj in from)
      if (obj == null)
      {
        result.Add(null);
      }
      else
      {
        var mapper = ObjectMapperManager.GetMapperImpl(
          obj.GetType(),
          obj.GetType(),
          MappingConfigurator
        );
        result.Add(mapper.Map(obj));
      }

    return result;
  }

  private ArrayList CopyToArrayListScalar(object from)
  {
    var result = new ArrayList(1);
    if (ShallowCopy)
    {
      result.Add(from);
      return result;
    }

    var mapper = ObjectMapperManager.GetMapperImpl(from.GetType(), from.GetType(), MappingConfigurator);
    result.Add(mapper.Map(from));
    return result;
  }

  private Array CopyScalarToArray(object scalar)
  {
    var result = Array.CreateInstance(TypeTo.GetElementType(), 1);
    result.SetValue(_subMapper.Mapper.Map(scalar), 0);
    return result;
  }
}