namespace EmitMapper.AST.Nodes;

using System.Reflection;
using System.Reflection.Emit;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

internal class AstWriteField : IAstNode
{
    public FieldInfo FieldInfo;

    public IAstRefOrAddr TargetObject;

    public IAstRefOrValue Value;

    public void Compile(CompilationContext context)
    {
        this.TargetObject.Compile(context);
        this.Value.Compile(context);
        CompilationHelper.PrepareValueOnStack(context, this.FieldInfo.FieldType, this.Value.ItemType);
        context.Emit(OpCodes.Stfld, this.FieldInfo);
    }
}