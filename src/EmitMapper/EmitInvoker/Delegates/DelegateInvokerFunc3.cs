namespace EmitMapper.EmitInvoker.Delegates;

/// <summary>
///   The delegate invoker func3.
/// </summary>
public abstract class DelegateInvokerFunc3 : DelegateInvokerBase
{
	/// <summary>
	///   Calls the func.
	/// </summary>
	/// <param name="param1">The param1.</param>
	/// <param name="param2">The param2.</param>
	/// <param name="param3">The param3.</param>
	/// <returns>An object.</returns>
	public abstract object CallFunc(object param1, object param2, object param3);
}