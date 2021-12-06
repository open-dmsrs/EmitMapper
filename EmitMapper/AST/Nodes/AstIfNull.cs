using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    /// <summary>
    /// Generates "value ?? ifNullValue" expression.
    /// </summary>

    /* Unmerged change from project 'EmitMapper (netstandard2.1)'
    Before:
        class AstIfNull : IAstRefOrValue
    After:
        class AstIfNull : IAstRefOrValue
    */
    internal class AstIfNull : IAstRefOrValue
    {

        /* Unmerged change from project 'EmitMapper (netstandard2.1)'
        Before:
                IAstRef _value;
                IAstRefOrValue _ifNullValue;
        After:
                IAstRef _value;
                IAstRefOrValue _ifNullValue;
        */
        private readonly IAstRef _value;
        private readonly IAstRefOrValue _ifNullValue;

        public Type ItemType => _value.ItemType;

        public AstIfNull(IAstRef value, IAstRefOrValue ifNullValue)
        {
            _value = value;
            _ifNullValue = ifNullValue;
            if (!_value.ItemType.IsAssignableFrom(_ifNullValue.ItemType))
            {
                throw new EmitMapperException("Incorrect ifnull expression");
            }
        }

        public void Compile(CompilationContext context)
        {
            Label ifNotNullLabel = context.ILGenerator.DefineLabel();
            _value.Compile(context);
            context.Emit(OpCodes.Dup);
            context.Emit(OpCodes.Brtrue_S, ifNotNullLabel);
            context.Emit(OpCodes.Pop);
            _ifNullValue.Compile(context);
            context.ILGenerator.MarkLabel(ifNotNullLabel);
        }
    }
}
