namespace EmitMapper.EmitInvoker.Methods;

/// <summary>
///   The method invoker func2.
/// </summary>
public abstract class MethodInvokerFunc2 : MethodInvokerBase
{
	/// <summary>
	///   Calls the func.
	/// </summary>
	/// <param name="param1">The param1.</param>
	/// <param name="param2">The param2.</param>
	/// <returns>An object.</returns>
	public abstract object CallFunc(object param1, object param2);
}