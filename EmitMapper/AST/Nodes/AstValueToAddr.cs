namespace EmitMapper.AST.Nodes;

using System;

using EmitMapper.AST.Interfaces;

internal class AstValueToAddr : IAstAddr
{
    public IAstValue Value;

    public AstValueToAddr(IAstValue value)
    {
        this.Value = value;
    }

    public Type ItemType => this.Value.ItemType;

    public void Compile(CompilationContext context)
    {
        var loc = context.ILGenerator.DeclareLocal(this.ItemType);
        new AstInitializeLocalVariable(loc).Compile(context);
        new AstWriteLocal { LocalIndex = loc.LocalIndex, LocalType = loc.LocalType, Value = this.Value }.Compile(
            context);
        new AstReadLocalAddr(loc).Compile(context);
    }
}