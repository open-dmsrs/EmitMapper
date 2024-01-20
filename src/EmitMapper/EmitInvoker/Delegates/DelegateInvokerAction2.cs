namespace EmitMapper.EmitInvoker.Delegates;

/// <summary>
///   The delegate invoker action2.
/// </summary>
public abstract class DelegateInvokerAction2 : DelegateInvokerBase
{
	/// <summary>
	///   Calls the action.
	/// </summary>
	/// <param name="param1">The param1.</param>
	/// <param name="param2">The param2.</param>
	public abstract void CallAction(object param1, object param2);
}