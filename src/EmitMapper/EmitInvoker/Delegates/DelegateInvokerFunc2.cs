namespace EmitMapper.EmitInvoker.Delegates;

/// <summary>
///   The delegate invoker func2.
/// </summary>
public abstract class DelegateInvokerFunc2 : DelegateInvokerBase
{
	/// <summary>
	///   Calls the func.
	/// </summary>
	/// <param name="param1">The param1.</param>
	/// <param name="param2">The param2.</param>
	/// <returns>An object.</returns>
	public abstract object? CallFunc(object? param1, object? param2);
}