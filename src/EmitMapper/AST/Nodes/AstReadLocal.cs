using System;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;
/// <summary>
/// The ast read local.
/// </summary>

internal class AstReadLocal : IAstStackItem
{
  public int LocalIndex;

  public Type LocalType;

  /// <summary>
  /// Initializes a new instance of the <see cref="AstReadLocal"/> class.
  /// </summary>
  public AstReadLocal()
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="AstReadLocal"/> class.
  /// </summary>
  /// <param name="loc">The loc.</param>
  public AstReadLocal(LocalVariableInfo loc)
  {
    LocalIndex = loc.LocalIndex;
    LocalType = loc.LocalType;
  }

  /// <summary>
  /// Gets the item type.
  /// </summary>
  public Type ItemType => LocalType;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="context">The context.</param>
  public virtual void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldloc, LocalIndex);
  }
}