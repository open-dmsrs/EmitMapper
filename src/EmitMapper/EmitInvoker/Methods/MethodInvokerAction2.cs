namespace EmitMapper.EmitInvoker.Methods;

/// <summary>
///   The method invoker action2.
/// </summary>
public abstract class MethodInvokerAction2 : MethodInvokerBase
{
	/// <summary>
	///   Calls the action.
	/// </summary>
	/// <param name="param1">The param1.</param>
	/// <param name="param2">The param2.</param>
	public abstract void CallAction(object param1, object param2);
}