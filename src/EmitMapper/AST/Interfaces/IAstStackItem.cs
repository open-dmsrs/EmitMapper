namespace EmitMapper.AST.Interfaces;

/// <summary>
///   The ast stack item interface.
/// </summary>
internal interface IAstStackItem : IAstNode
{
	/// <summary>
	///   Gets the item type.
	/// </summary>
	Type ItemType { get; }
}