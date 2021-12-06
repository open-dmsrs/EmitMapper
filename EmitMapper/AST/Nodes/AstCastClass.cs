namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

internal class AstCastclass : IAstRefOrValue
{
    protected Type TargetType;

    protected IAstRefOrValue Value;

    public AstCastclass(IAstRefOrValue value, Type targetType)
    {
        this.Value = value;
        this.TargetType = targetType;
    }

    #region IAstStackItem Members

    public Type ItemType => this.TargetType;

    #endregion

    #region IAstNode Members

    public virtual void Compile(CompilationContext context)
    {
        if (this.Value.ItemType != this.TargetType)
        {
            if (!this.Value.ItemType.IsValueType && !this.TargetType.IsValueType)
            {
                this.Value.Compile(context);
                context.Emit(OpCodes.Castclass, this.TargetType);
                return;
            }

            if (this.TargetType.IsValueType && !this.Value.ItemType.IsValueType)
            {
                new AstUnbox { RefObj = (IAstRef)this.Value, UnboxedType = this.TargetType }.Compile(context);
                return;
            }

            throw new EmitMapperException();
        }

        this.Value.Compile(context);
    }

    #endregion
}

internal class AstCastclassRef : AstCastclass, IAstRef
{
    public AstCastclassRef(IAstRefOrValue value, Type targetType)
        : base(value, targetType)
    {
    }

    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsRef(this.ItemType);
        base.Compile(context);
    }
}

internal class AstCastclassValue : AstCastclass, IAstValue
{
    public AstCastclassValue(IAstRefOrValue value, Type targetType)
        : base(value, targetType)
    {
    }

    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsValue(this.ItemType);
        base.Compile(context);
    }
}