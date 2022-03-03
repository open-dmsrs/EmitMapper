namespace EmitMapper.EmitInvoker.Delegates;

/// <summary>
///   The delegate invoker func1.
/// </summary>
public abstract class DelegateInvokerFunc1 : DelegateInvokerBase
{
  /// <summary>
  ///   Calls the func.
  /// </summary>
  /// <param name="param1">The param1.</param>
  /// <returns>An object.</returns>
  public abstract object CallFunc(object param1);
}