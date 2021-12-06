using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstWriteField : IAstNode
    {
        public IAstRefOrAddr TargetObject;
        public IAstRefOrValue Value;
        public FieldInfo FieldInfo;

        public void Compile(CompilationContext context)
        {
            TargetObject.Compile(context);
            Value.Compile(context);
            CompilationHelper.PrepareValueOnStack(context, FieldInfo.FieldType, Value.ItemType);
            context.Emit(OpCodes.Stfld, FieldInfo);
        }
    }
}