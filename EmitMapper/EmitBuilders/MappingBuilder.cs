namespace EmitMapper.EmitBuilders;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Nodes;
using EmitMapper.Conversion;
using EmitMapper.MappingConfiguration;
using EmitMapper.Utils;

internal class MappingBuilder
{
    private readonly IMappingConfigurator _mappingConfigurator;

    private readonly ObjectMapperManager _objectsMapperManager;

    private readonly TypeBuilder _typeBuilder;

    public readonly List<object> StoredObjects;

    private Type _from;

    private Type _to;

    public MappingBuilder(
        ObjectMapperManager objectsMapperManager,
        Type from,
        Type to,
        TypeBuilder typeBuilder,
        IMappingConfigurator mappingConfigurator)
    {
        this._objectsMapperManager = objectsMapperManager;
        this._from = from;
        this._to = to;
        this._typeBuilder = typeBuilder;

        this.StoredObjects = new List<object>();
        this._mappingConfigurator = mappingConfigurator;
    }

    public void BuildCopyImplMethod()
    {
        if (ReflectionUtils.IsNullable(this._from))
            this._from = Nullable.GetUnderlyingType(this._from);
        if (ReflectionUtils.IsNullable(this._to))
            this._to = Nullable.GetUnderlyingType(this._to);

        var methodBuilder = this._typeBuilder.DefineMethod(
            "MapImpl",
            MethodAttributes.Public | MethodAttributes.Virtual,
            typeof(object),
            new[] { typeof(object), typeof(object), typeof(object) });

        var ilGen = methodBuilder.GetILGenerator();
        var compilationContext = new CompilationContext(ilGen);

        var mapperAst = new AstComplexNode();

        var locFrom = ilGen.DeclareLocal(this._from);
        var locTo = ilGen.DeclareLocal(this._to);
        var locState = ilGen.DeclareLocal(typeof(object));
        LocalBuilder locException = null;

        mapperAst.Nodes.Add(BuilderUtils.InitializeLocal(locFrom, 1));
        mapperAst.Nodes.Add(BuilderUtils.InitializeLocal(locTo, 2));
        mapperAst.Nodes.Add(BuilderUtils.InitializeLocal(locState, 3));

#if DEBUG
        locException = compilationContext.ILGenerator.DeclareLocal(typeof(Exception));
#endif

        var mappingOperations = this._mappingConfigurator.GetMappingOperations(this._from, this._to);
        var staticConverter = this._mappingConfigurator.GetStaticConvertersManager();
        mapperAst.Nodes.Add(
            new MappingOperationsProcessor
                {
                    LocException = locException,
                    LocFrom = locFrom,
                    LocState = locState,
                    LocTo = locTo,
                    ObjectsMapperManager = this._objectsMapperManager,
                    CompilationContext = compilationContext,
                    StoredObjects = this.StoredObjects,
                    Operations = mappingOperations,
                    MappingConfigurator = this._mappingConfigurator,
                    RootOperation = this._mappingConfigurator.GetRootMappingOperation(this._from, this._to),
                    StaticConvertersManager = staticConverter ?? StaticConvertersManager.DefaultInstance
                }.ProcessOperations());
        mapperAst.Nodes.Add(
            new AstReturn { ReturnType = typeof(object), ReturnValue = AstBuildHelper.ReadLocalRV(locTo) });

        mapperAst.Compile(compilationContext);
    }
}