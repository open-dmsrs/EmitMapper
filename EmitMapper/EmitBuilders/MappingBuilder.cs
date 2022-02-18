using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Nodes;
using EmitMapper.Conversion;
using EmitMapper.Mappers;
using EmitMapper.MappingConfiguration;
using EmitMapper.Utils;

namespace EmitMapper.EmitBuilders;

internal class MappingBuilder
{
  public readonly List<object> StoredObjects;

  private readonly IMappingConfigurator _mappingConfigurator;

  private readonly ObjectMapperManager _objectsMapperManager;

  private readonly TypeBuilder _typeBuilder;

  private Type _from;

  private Type _to;

  public MappingBuilder(
    ObjectMapperManager objectsMapperManager,
    Type from,
    Type to,
    TypeBuilder typeBuilder,
    IMappingConfigurator mappingConfigurator)
  {
    _objectsMapperManager = objectsMapperManager;
    _from = from;
    _to = to;
    _typeBuilder = typeBuilder;

    StoredObjects = new List<object>();
    _mappingConfigurator = mappingConfigurator;
  }

  public void BuildCopyImplMethod()
  {
    if (ReflectionHelper.IsNullable(_from))
      _from = _from.GetUnderlyingTypeCache();
    if (ReflectionHelper.IsNullable(_to))
      _to = _to.GetUnderlyingTypeCache();

    var methodBuilder = _typeBuilder.DefineMethod(
      nameof(ObjectsMapperBaseImpl.MapImpl),
      MethodAttributes.Public | MethodAttributes.Virtual,
      Metadata<object>.Type,
      new[] { Metadata<object>.Type, Metadata<object>.Type, Metadata<object>.Type });

    var ilGen = methodBuilder.GetILGenerator();
    var compilationContext = new CompilationContext(ilGen);

    var mapperAst = new AstComplexNode();

    var locFrom = ilGen.DeclareLocal(_from);
    var locTo = ilGen.DeclareLocal(_to);
    var locState = ilGen.DeclareLocal(Metadata<object>.Type);
    LocalBuilder locException = null;

    mapperAst.Nodes.Add(BuilderUtils.InitializeLocal(locFrom, 1));
    mapperAst.Nodes.Add(BuilderUtils.InitializeLocal(locTo, 2));
    mapperAst.Nodes.Add(BuilderUtils.InitializeLocal(locState, 3));

#if DEBUG
    locException = compilationContext.ILGenerator.DeclareLocal(Metadata<Exception>.Type);
#endif

    var mappingOperations = _mappingConfigurator.GetMappingOperations(_from, _to);
    var staticConverter = _mappingConfigurator.GetStaticConvertersManager();
    mapperAst.Nodes.Add(
      new MappingOperationsProcessor
      {
        LocException = locException,
        LocFrom = locFrom,
        LocState = locState,
        LocTo = locTo,
        ObjectsMapperManager = _objectsMapperManager,
        CompilationContext = compilationContext,
        StoredObjects = StoredObjects,
        Operations = mappingOperations,
        MappingConfigurator = _mappingConfigurator,
        RootOperation = _mappingConfigurator.GetRootMappingOperation(_from, _to),
        StaticConvertersManager = staticConverter ?? StaticConvertersManager.DefaultInstance
      }.ProcessOperations());
    mapperAst.Nodes.Add(
      new AstReturn { ReturnType = Metadata<object>.Type, ReturnValue = AstBuildHelper.ReadLocalRV(locTo) });

    mapperAst.Compile(compilationContext);
  }
}