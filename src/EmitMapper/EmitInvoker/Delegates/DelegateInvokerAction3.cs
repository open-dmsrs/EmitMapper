namespace EmitMapper.EmitInvoker.Delegates;

/// <summary>
///   The delegate invoker action3.
/// </summary>
public abstract class DelegateInvokerAction3 : DelegateInvokerBase
{
  /// <summary>
  ///   Calls the action.
  /// </summary>
  /// <param name="param1">The param1.</param>
  /// <param name="param2">The param2.</param>
  /// <param name="param3">The param3.</param>
  public abstract void CallAction(object param1, object param2, object param3);
}