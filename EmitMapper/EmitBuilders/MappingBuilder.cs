namespace EmitMapper.EmitBuilders;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Nodes;
using EmitMapper.Conversion;
using EmitMapper.Utils;

internal class MappingBuilder
{
    private readonly IMappingConfigurator mappingConfigurator;

    private readonly ObjectMapperManager objectsMapperManager;

    private readonly TypeBuilder typeBuilder;

    private Type from;

    public List<object> storedObjects;

    private Type to;

    public MappingBuilder(
        ObjectMapperManager objectsMapperManager,
        Type from,
        Type to,
        TypeBuilder typeBuilder,
        IMappingConfigurator mappingConfigurator)
    {
        this.objectsMapperManager = objectsMapperManager;
        this.from = from;
        this.to = to;
        this.typeBuilder = typeBuilder;

        /* Unmerged change from project 'EmitMapper (netstandard2.1)'
        Before:
                    this.storedObjects = new List<object>();
        After:
                    this.storedObjects = new List<object>();
        */
        this.storedObjects = new List<object>();
        this.mappingConfigurator = mappingConfigurator;
    }

    public void BuildCopyImplMethod()
    {
        if (ReflectionUtils.IsNullable(this.from))
            this.from = Nullable.GetUnderlyingType(this.from);
        if (ReflectionUtils.IsNullable(this.to))
            this.to = Nullable.GetUnderlyingType(this.to);

        var methodBuilder = this.typeBuilder.DefineMethod(
            "MapImpl",
            MethodAttributes.Public | MethodAttributes.Virtual,
            typeof(object),
            new[] { typeof(object), typeof(object), typeof(object) });

        var ilGen = methodBuilder.GetILGenerator();
        var compilationContext = new CompilationContext(ilGen);

        var mapperAst = new AstComplexNode();

        var locFrom = ilGen.DeclareLocal(this.from);
        var locTo = ilGen.DeclareLocal(this.to);
        var locState = ilGen.DeclareLocal(typeof(object));
        LocalBuilder locException = null;

        mapperAst.Nodes.Add(BuilderUtils.InitializeLocal(locFrom, 1));
        mapperAst.Nodes.Add(BuilderUtils.InitializeLocal(locTo, 2));
        mapperAst.Nodes.Add(BuilderUtils.InitializeLocal(locState, 3));

#if DEBUG
        locException = compilationContext.ILGenerator.DeclareLocal(typeof(Exception));
#endif

        /* Unmerged change from project 'EmitMapper (netstandard2.1)'
        Before:
                    var mappingOperations = mappingConfigurator.GetMappingOperations(from, to);
        After:
                    var mappingOperations = mappingConfigurator.GetMappingOperations(from, to);
        */
        var mappingOperations = this.mappingConfigurator.GetMappingOperations(this.from, this.to);
        var staticConverter = this.mappingConfigurator.GetStaticConvertersManager();
        mapperAst.Nodes.Add(
            new MappingOperationsProcessor
            {
                locException = locException,
                locFrom = locFrom,
                locState = locState,
                locTo = locTo,
                objectsMapperManager = this.objectsMapperManager,
                compilationContext = compilationContext,
                storedObjects = this.storedObjects,
                operations = mappingOperations,
                mappingConfigurator = this.mappingConfigurator,
                rootOperation = this.mappingConfigurator.GetRootMappingOperation(this.from, this.to),
                staticConvertersManager = staticConverter ?? StaticConvertersManager.DefaultInstance
            }.ProcessOperations());
        mapperAst.Nodes.Add(
            new AstReturn { ReturnType = typeof(object), ReturnValue = AstBuildHelper.ReadLocalRV(locTo) });

        mapperAst.Compile(compilationContext);
    }
}