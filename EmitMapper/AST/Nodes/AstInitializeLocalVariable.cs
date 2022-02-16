namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstInitializeLocalVariable : IAstNode
{
  public int LocalIndex;

  public Type LocalType;

  public AstInitializeLocalVariable()
  {
  }

  public AstInitializeLocalVariable(LocalVariableInfo loc)
  {
    LocalType = loc.LocalType;
    LocalIndex = loc.LocalIndex;
  }

  public void Compile(CompilationContext context)
  {
    if (LocalType.IsValueType)
    {
      context.Emit(OpCodes.Ldloca, LocalIndex);
      context.Emit(OpCodes.Initobj, LocalType);
    }
  }
}