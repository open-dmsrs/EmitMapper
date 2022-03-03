using System.Collections.Generic;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.Utils;

namespace EmitMapper.EmitBuilders;

internal static class BuilderUtils
{
  /// <summary>
  ///   Copies an argument to local variable
  /// </summary>
  /// <param name="loc"></param>
  /// <param name="argIndex"></param>
  /// <returns></returns>
  public static IAstNode InitializeLocal(LocalBuilder loc, int argIndex)
  {
    return new AstComplexNode
    {
      Nodes = new List<IAstNode>
      {
        new AstInitializeLocalVariable(loc),
        new AstWriteLocal
        {
          LocalIndex = loc.LocalIndex,
          LocalType = loc.LocalType,
          Value = AstBuildHelper.ReadArgumentRV(argIndex, Metadata<object>.Type)
        }
      }
    };
  }
}