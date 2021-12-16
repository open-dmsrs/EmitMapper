using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstUnbox : IAstValue
{
    public IAstRef RefObj;

    public Type UnboxedType;

    public Type ItemType => UnboxedType;

    public void Compile(CompilationContext context)
    {
        RefObj.Compile(context);
        context.Emit(OpCodes.Unbox_Any, UnboxedType);
    }
}