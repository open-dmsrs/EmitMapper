namespace EmitMapper.EmitInvoker.Methods;

/// <summary>
///   The method invoker func1.
/// </summary>
public abstract class MethodInvokerFunc1 : MethodInvokerBase
{
	/// <summary>
	///   Calls the func.
	/// </summary>
	/// <param name="param1">The param1.</param>
	/// <returns>An object.</returns>
	public abstract object CallFunc(object param1);
}