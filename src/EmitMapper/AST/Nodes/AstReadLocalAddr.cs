using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstReadLocalAddr : AstReadLocal, IAstAddr
{
  public AstReadLocalAddr(LocalVariableInfo loc)
  {
    LocalIndex = loc.LocalIndex;
    LocalType = loc.LocalType.MakeByRefType();
  }

  public override void Compile(CompilationContext context)
  {
    // CompilationHelper.CheckIsValue(itemType);
    context.Emit(OpCodes.Ldloca, LocalIndex);
  }
}