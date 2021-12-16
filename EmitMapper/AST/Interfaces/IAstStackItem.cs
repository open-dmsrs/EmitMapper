using System;

namespace EmitMapper.AST.Interfaces;

internal interface IAstStackItem : IAstNode
{
    Type ItemType { get; }
}