
/* Unmerged change from project 'EmitMapper (netstandard2.1)'
Before:
using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
After:
using EmitMapper.AST.Helpers;
using EmitMapper.Reflection.Interfaces;
using System;
using System.AST.Interfaces;
*/
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstReturn : IAstNode, IAstAddr
    {
        public Type ReturnType;
        public IAstRefOrValue ReturnValue;

        public void Compile(CompilationContext context)
        {
            ReturnValue.Compile(context);
            CompilationHelper.PrepareValueOnStack(context, ReturnType, ReturnValue.ItemType);
            context.Emit(OpCodes.Ret);
        }

        public Type ItemType => ReturnType;
    }
}