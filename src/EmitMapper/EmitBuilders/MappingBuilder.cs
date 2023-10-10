namespace EmitMapper.EmitBuilders;

/// <summary>
///   The mapping builder.
/// </summary>
internal class MappingBuilder
{
  public readonly List<object> StoredObjects;

  private readonly IMappingConfigurator _mappingConfigurator;

  private readonly Mapper _objectsMapperManager;

  private readonly TypeBuilder _typeBuilder;

  private Type _from;

  private Type _to;

  /// <summary>
  ///   Initializes a new instance of the <see cref="MappingBuilder" /> class.
  /// </summary>
  /// <param name="objectsMapperManager">The objects mapper manager.</param>
  /// <param name="from">The from.</param>
  /// <param name="to">The to.</param>
  /// <param name="typeBuilder">The type builder.</param>
  /// <param name="mappingConfigurator">The mapping configurator.</param>
  public MappingBuilder(
    Mapper objectsMapperManager,
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

  /// <summary>
  ///   Builds the copy impl method.
  /// </summary>
  public void BuildCopyImplMethod()
  {
    if (ReflectionHelper.IsNullable(_from))
      _from = _from.GetUnderlyingTypeCache();

    if (ReflectionHelper.IsNullable(_to))
      _to = _to.GetUnderlyingTypeCache();

    var methodBuilder = _typeBuilder.DefineMethod(
      nameof(MapperBase.MapImpl),
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