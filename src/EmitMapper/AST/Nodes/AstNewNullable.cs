using System;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;
/// <summary>
/// The ast new nullable.
/// </summary>

internal class AstNewNullable : IAstValue
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AstNewNullable"/> class.
  /// </summary>
  /// <param name="nullableType">The nullable type.</param>
  public AstNewNullable(Type nullableType)
  {
    ItemType = nullableType;
  }

  /// <summary>
  /// Gets the item type.
  /// </summary>
  public Type ItemType { get; }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="context">The context.</param>
  /// <exception cref="NotImplementedException"></exception>
  public void Compile(CompilationContext context)
  {
    throw new NotImplementedException();
  }
}