using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read local addr.
/// </summary>
internal class AstReadLocalAddr : AstReadLocal, IAstAddr
{
  /// <summary>
  ///   Initializes a new instance of the <see cref="AstReadLocalAddr" /> class.
  /// </summary>
  /// <param name="loc">The loc.</param>
  public AstReadLocalAddr(LocalVariableInfo loc)
  {
    LocalIndex = loc.LocalIndex;
    LocalType = loc.LocalType.MakeByRefType();
  }

  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public override void Compile(CompilationContext context)
  {
    // CompilationHelper.CheckIsValue(itemType);
    context.Emit(OpCodes.Ldloca, LocalIndex);
  }
}