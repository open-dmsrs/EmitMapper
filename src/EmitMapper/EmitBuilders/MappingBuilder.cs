namespace EmitMapper.EmitBuilders;

/// <summary>
///   The mapping builder.
/// </summary>
internal class MappingBuilder
{
	public readonly List<object> StoredObjects;

	private readonly IMappingConfigurator mappingConfigurator;

	private readonly Mapper objectsMapperManager;

	private readonly TypeBuilder typeBuilder;

	private Type from;

	private Type to;

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
		this.objectsMapperManager = objectsMapperManager;
		this.from = from;
		this.to = to;
		this.typeBuilder = typeBuilder;

		StoredObjects = new List<object>();
		this.mappingConfigurator = mappingConfigurator;
	}

	/// <summary>
	///   Builds the copy impl method.
	/// </summary>
	public void BuildCopyImplMethod()
	{
		if (ReflectionHelper.IsNullable(from))
		{
			from = from.GetUnderlyingTypeCache();
		}

		if (ReflectionHelper.IsNullable(to))
		{
			to = to.GetUnderlyingTypeCache();
		}

		var methodBuilder = typeBuilder.DefineMethod(
		  nameof(MapperBase.MapImpl),
		  MethodAttributes.Public | MethodAttributes.Virtual,
		  Metadata<object>.Type,
		  new[] { Metadata<object>.Type, Metadata<object>.Type, Metadata<object>.Type });

		var ilGen = methodBuilder.GetILGenerator();
		var compilationContext = new CompilationContext(ilGen);

		var mapperAst = new AstComplexNode();

		var locFrom = ilGen.DeclareLocal(from);
		var locTo = ilGen.DeclareLocal(to);
		var locState = ilGen.DeclareLocal(Metadata<object>.Type);
		LocalBuilder? locException = null;

		mapperAst.Nodes.Add(BuilderUtils.InitializeLocal(locFrom, 1));
		mapperAst.Nodes.Add(BuilderUtils.InitializeLocal(locTo, 2));
		mapperAst.Nodes.Add(BuilderUtils.InitializeLocal(locState, 3));

#if DEBUG
		locException = compilationContext.ILGenerator.DeclareLocal(Metadata<Exception>.Type);
#endif

		var mappingOperations = mappingConfigurator.GetMappingOperations(from, to);
		var staticConverter = mappingConfigurator.GetStaticConvertersManager();

		mapperAst.Nodes.Add(
		  new MappingOperationsProcessor
		  {
			  LocException = locException,
			  LocFrom = locFrom,
			  LocState = locState,
			  LocTo = locTo,
			  ObjectsMapperManager = objectsMapperManager,
			  CompilationContext = compilationContext,
			  StoredObjects = StoredObjects,
			  Operations = mappingOperations,
			  MappingConfigurator = mappingConfigurator,
			  RootOperation = mappingConfigurator.GetRootMappingOperation(from, to),
			  StaticConvertersManager = staticConverter ?? StaticConvertersManager.DefaultInstance
		  }.ProcessOperations());

		mapperAst.Nodes.Add(
		  new AstReturn { ReturnType = Metadata<object>.Type, ReturnValue = AstBuildHelper.ReadLocalRV(locTo) });

		mapperAst.Compile(compilationContext);
	}
}