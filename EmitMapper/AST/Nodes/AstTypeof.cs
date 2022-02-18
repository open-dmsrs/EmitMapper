using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.AST.Nodes;

internal class AstTypeof : IAstRef
{
  public Type Type;

  public Type ItemType => Metadata<Type>.Type;

  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldtoken, Type);
    context.EmitCall(OpCodes.Call, Metadata<Type>.Type.GetMethodCache(nameof(Type.GetTypeFromHandle)));
  }
}