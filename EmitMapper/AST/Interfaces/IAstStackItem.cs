namespace EmitMapper.AST.Interfaces;

using System;

internal interface IAstStackItem : IAstNode
{
    Type ItemType { get; }
}