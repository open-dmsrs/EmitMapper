namespace EmitMapper.AST.Nodes;

using System;
using System.Linq;
using System.Reflection;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

internal class AstNewObject : IAstRef
{
    public IAstStackItem[] ConstructorParams;

    public Type ObjectType;

    public AstNewObject()
    {
    }

    public AstNewObject(Type objectType, IAstStackItem[] constructorParams)
    {
        this.ObjectType = objectType;
        this.ConstructorParams = constructorParams;
    }

    #region IAstStackItem Members

    public Type ItemType => this.ObjectType;

    #endregion

    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        if (ReflectionUtils.IsNullable(this.ObjectType))
        {
            IAstRefOrValue underlyingValue;
            var underlyingType = Nullable.GetUnderlyingType(this.ObjectType);
            if (this.ConstructorParams == null || this.ConstructorParams.Length == 0)
            {
                var temp = context.ILGenerator.DeclareLocal(underlyingType);
                new AstInitializeLocalVariable(temp).Compile(context);
                underlyingValue = AstBuildHelper.ReadLocalRV(temp);
            }
            else
            {
                underlyingValue = (IAstValue)this.ConstructorParams[0];
            }

            var constructor = this.ObjectType.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance,
                null,
                new[] { underlyingType },
                null);

            underlyingValue.Compile(context);
            context.EmitNewObject(constructor);
        }
        else
        {
            Type[] types;
            if (this.ConstructorParams == null || this.ConstructorParams.Length == 0)
            {
                types = new Type[0];
            }
            else
            {
                types = this.ConstructorParams.Select(c => c.ItemType).ToArray();
                foreach (var p in this.ConstructorParams)
                    p.Compile(context);
            }

            var ci = this.ObjectType.GetConstructor(types);
            if (ci != null)
            {
                context.EmitNewObject(ci);
            }
            else if (this.ObjectType.IsValueType)
            {
                var temp = context.ILGenerator.DeclareLocal(this.ObjectType);
                new AstInitializeLocalVariable(temp).Compile(context);
                AstBuildHelper.ReadLocalRV(temp).Compile(context);
            }
            else
            {
                throw new Exception(
                    string.Format(
                        "Constructor for types [{0}] not found in {1}",
                        types.ToCSV(","),
                        this.ObjectType.FullName));
            }
        }
    }

    #endregion
}