using System;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast initialize local variable.
/// </summary>
internal class AstInitializeLocalVariable : IAstNode
{
  public int LocalIndex;

  public Type LocalType;

  /// <summary>
  ///   Initializes a new instance of the <see cref="AstInitializeLocalVariable" /> class.
  /// </summary>
  public AstInitializeLocalVariable()
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="AstInitializeLocalVariable" /> class.
  /// </summary>
  /// <param name="loc">The loc.</param>
  public AstInitializeLocalVariable(LocalVariableInfo loc)
  {
    LocalType = loc.LocalType;
    LocalIndex = loc.LocalIndex;
  }

  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    if (LocalType.IsValueType)
    {
      context.Emit(OpCodes.Ldloca, LocalIndex);
      context.Emit(OpCodes.Initobj, LocalType);
    }
  }
}