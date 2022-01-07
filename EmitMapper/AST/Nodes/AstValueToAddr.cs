using System;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstValueToAddr : IAstAddr
{
  public IAstValue Value;

  public AstValueToAddr(IAstValue value)
  {
    Value = value;
  }

  public Type ItemType => Value.ItemType;

  public void Compile(CompilationContext context)
  {
    var loc = context.ILGenerator.DeclareLocal(ItemType);
    new AstInitializeLocalVariable(loc).Compile(context);
    new AstWriteLocal { LocalIndex = loc.LocalIndex, LocalType = loc.LocalType, Value = Value }.Compile(
      context);
    new AstReadLocalAddr(loc).Compile(context);
  }
}