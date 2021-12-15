namespace EmitMapper.Mappers;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.MappingConfiguration;

/// <summary>
///     Mapper for collections. It can copy Array, List&lt;&gt;, ArrayList collections.
///     Collection type in source object and destination object can differ.
/// </summary>
public class MapperForCollectionImpl : CustomMapperImpl
{
    private ObjectsMapperDescr _subMapper;

    protected MapperForCollectionImpl()
        : base(null, null, null, null, null)
    {
    }

    /// <summary>
    ///     Creates an instance of Mapper for collections.
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
        var tb = DynamicAssemblyManager.DefineType("GenericListInv_" + mapperName, typeof(MapperForCollectionImpl));

        if (typeTo.IsGenericType && typeTo.GetGenericTypeDefinition() == typeof(List<>))
        {
            var methodBuilder = tb.DefineMethod(
                nameof(CopyToListInvoke),
                MethodAttributes.Family | MethodAttributes.Virtual,
                typeof(object),
                new[] { typeof(IEnumerable) });

            InvokeCopyImpl(typeTo, nameof(CopyToList)).Compile(new CompilationContext(methodBuilder.GetILGenerator()));

            methodBuilder = tb.DefineMethod(
                nameof(CopyToListScalarInvoke),
                MethodAttributes.Family | MethodAttributes.Virtual,
                typeof(object),
                new[] { typeof(object) });

            InvokeCopyImpl(typeTo, nameof(CopyToListScalar))
                .Compile(new CompilationContext(methodBuilder.GetILGenerator()));
        }

        var result = (MapperForCollectionImpl)Activator.CreateInstance(tb.CreateType());
        result.Initialize(objectMapperManager, typeFrom, typeTo, mappingConfigurator, null);
        result._subMapper = subMapper;

        return result;
    }

    /// <summary>
    ///     Returns true if specified type is supported by this Mapper
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    internal static bool IsSupportedType(Type type)
    {
        return type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)
                            || type == typeof(ArrayList) || typeof(IList).IsAssignableFrom(type)
                            || typeof(IList<>).IsAssignableFrom(type);
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
        var mi = typeof(MapperForCollectionImpl).GetMethod(copyMethodName, BindingFlags.Instance | BindingFlags.Public)
            .MakeGenericMethod(ExtractElementType(copiedObjectType));

        return new AstReturn
                   {
                       ReturnType = typeof(object),
                       ReturnValue = AstBuildHelper.CallMethod(
                           mi,
                           AstBuildHelper.ReadThis(typeof(MapperForCollectionImpl)),
                           new List<IAstStackItem>
                               {
                                   new AstReadArgumentRef { ArgumentIndex = 1, ArgumentType = typeof(object) }
                               })
                   };
    }

    private static Type ExtractElementType(Type collection)
    {
        if (collection.IsArray)
            return collection.GetElementType();
        if (collection == typeof(ArrayList))
            return typeof(object);
        if (collection.IsGenericType && collection.GetGenericTypeDefinition() == typeof(List<>))
            return collection.GetGenericArguments()[0];
        return null;
    }

    /// <summary>
    ///     Copies object properties and members of "from" to object "to"
    /// </summary>
    /// <param name="from">Source object</param>
    /// <param name="to">Destination object</param>
    /// <param name="state"></param>
    /// <returns>Destination object</returns>
    public override object MapImpl(object from, object to, object state)
    {
        if (to == null && this.TargetConstructor != null)
            to = this.TargetConstructor.CallFunc();

        if (this.TypeTo.IsArray)
        {
            if (from is IEnumerable)
                return this.CopyToArray((IEnumerable)from);
            return this.CopyScalarToArray(from);
        }

        if (this.TypeTo.IsGenericType && this.TypeTo.GetGenericTypeDefinition() == typeof(List<>))
        {
            if (from is IEnumerable)
                return this.CopyToListInvoke((IEnumerable)from);
            return this.CopyToListScalarInvoke(from);
        }

        if (this.TypeTo == typeof(ArrayList))
        {
            if (from is IEnumerable)
                return this.CopyToArrayList((IEnumerable)from);
            return this.CopyToArrayListScalar(from);
        }

        if (typeof(IList).IsAssignableFrom(this.TypeTo))
            return this.CopyToIList((IList)to, from);
        return null;
    }

    /// <summary>
    ///     Copies object properties and members of "from" to object "to"
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
            result.Add((T)this._subMapper.Mapper.Map(obj));
        return result;
    }

    protected List<T> CopyToListScalar<T>(object from)
    {
        var result = new List<T>(1) { (T)this._subMapper.Mapper.Map(from) };
        return result;
    }

    private object CopyToIList(IList iList, object from)
    {
        if (iList == null)
            iList = (IList)Activator.CreateInstance(this.TypeTo);
        foreach (var obj in from is IEnumerable fromEnumerable ? fromEnumerable : new[] { from })
        {
            if (obj == null)
                iList.Add(null);
            if (this.RootOperation == null || this.RootOperation.ShallowCopy)
            {
                iList.Add(obj);
            }
            else
            {
                var mapper = this.ObjectMapperManager.GetMapperImpl(
                    obj.GetType(),
                    obj.GetType(),
                    this.MappingConfigurator);
                iList.Add(mapper.Map(obj));
            }
        }

        return iList;
    }

    private Array CopyToArray(IEnumerable from)
    {
        if (from is ICollection)
        {
            var result = Array.CreateInstance(this.TypeTo.GetElementType(), ((ICollection)from).Count);
            var idx = 0;
            foreach (var obj in from)
                result.SetValue(this._subMapper.Mapper.Map(obj), idx++);
            return result;
        }
        else
        {
            var result = new ArrayList();
            foreach (var obj in from)
                result.Add(obj);
            return result.ToArray(this.TypeTo.GetElementType());
        }
    }

    private ArrayList CopyToArrayList(IEnumerable from)
    {
        if (this.ShallowCopy)
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
                var mapper = this.ObjectMapperManager.GetMapperImpl(
                    obj.GetType(),
                    obj.GetType(),
                    this.MappingConfigurator);
                result.Add(mapper.Map(obj));
            }

        return result;
    }

    private ArrayList CopyToArrayListScalar(object from)
    {
        var result = new ArrayList(1);
        if (this.ShallowCopy)
        {
            result.Add(from);
            return result;
        }

        var mapper = this.ObjectMapperManager.GetMapperImpl(from.GetType(), from.GetType(), this.MappingConfigurator);
        result.Add(mapper.Map(from));
        return result;
    }

    private Array CopyScalarToArray(object scalar)
    {
        var result = Array.CreateInstance(this.TypeTo.GetElementType(), 1);
        result.SetValue(this._subMapper.Mapper.Map(scalar), 0);
        return result;
    }
}