using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Nodes;
using EmitMapper.Conversion;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitMapper.EmitBuilders
{

    /* Unmerged change from project 'EmitMapper (netstandard2.1)'
    Before:
        class MappingBuilder
    After:
        class MappingBuilder
    */
    internal class MappingBuilder
    {

        /* Unmerged change from project 'EmitMapper (netstandard2.1)'
        Before:
                Type from;
                Type to;
                TypeBuilder typeBuilder;
        After:
                Type from;
                Type to;
                TypeBuilder typeBuilder;
        */
        private Type from;
        private Type to;
        private readonly TypeBuilder typeBuilder;
        public List<object> storedObjects;

        /* Unmerged change from project 'EmitMapper (netstandard2.1)'
        Before:
                IMappingConfigurator mappingConfigurator;
                ObjectMapperManager objectsMapperManager;
        After:
                IMappingConfigurator mappingConfigurator;
                ObjectMapperManager objectsMapperManager;
        */
        private readonly IMappingConfigurator mappingConfigurator;
        private readonly ObjectMapperManager objectsMapperManager;

        public MappingBuilder(
            ObjectMapperManager objectsMapperManager,
            Type from,
            Type to,
            TypeBuilder typeBuilder,
            IMappingConfigurator mappingConfigurator
            )
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
            storedObjects = new List<object>();
            this.mappingConfigurator = mappingConfigurator;
        }

        public void BuildCopyImplMethod()
        {
            if (ReflectionUtils.IsNullable(from))
            {
                from = Nullable.GetUnderlyingType(from);
            }
            if (ReflectionUtils.IsNullable(to))
            {
                to = Nullable.GetUnderlyingType(to);
            }

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                "MapImpl",
                MethodAttributes.FamORAssem | MethodAttributes.Virtual,
                typeof(object),
                new Type[] { typeof(object), typeof(object), typeof(object) }
                );

            ILGenerator ilGen = methodBuilder.GetILGenerator();
            CompilationContext compilationContext = new CompilationContext(ilGen);

            AstComplexNode mapperAst = new AstComplexNode();

            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                        var locFrom = ilGen.DeclareLocal(from);
                        var locTo = ilGen.DeclareLocal(to);
                        var locState = ilGen.DeclareLocal(typeof(object));
            After:
                        var locFrom = ilGen.DeclareLocal(from);
                        var locTo = ilGen.DeclareLocal(to);
                        var locState = ilGen.DeclareLocal(typeof(object));
            */
            LocalBuilder locFrom = ilGen.DeclareLocal(from);
            LocalBuilder locTo = ilGen.DeclareLocal(to);
            LocalBuilder locState = ilGen.DeclareLocal(typeof(object));
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
            IMappingOperation[] mappingOperations = mappingConfigurator.GetMappingOperations(from, to);
            StaticConvertersManager staticConverter = mappingConfigurator.GetStaticConvertersManager();
            mapperAst.Nodes.Add(
                new MappingOperationsProcessor()
                {
                    locException = locException,
                    locFrom = locFrom,
                    locState = locState,
                    locTo = locTo,
                    objectsMapperManager = objectsMapperManager,
                    compilationContext = compilationContext,
                    storedObjects = storedObjects,
                    operations = mappingOperations,
                    mappingConfigurator = mappingConfigurator,
                    rootOperation = mappingConfigurator.GetRootMappingOperation(from, to),
                    staticConvertersManager = staticConverter ?? StaticConvertersManager.DefaultInstance
                }.ProcessOperations()
            );
            mapperAst.Nodes.Add(
                new AstReturn()
                {
                    ReturnType = typeof(object),
                    ReturnValue = AstBuildHelper.ReadLocalRV(locTo)
                }
            );

            mapperAst.Compile(compilationContext);
        }
    }
}
