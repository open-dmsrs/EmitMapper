using System.Linq.Expressions;

namespace EmitMapper;

/// <summary>
///   The interface is implemented by the compiled delegate Target if `CompilerFlags.EnableDelegateDebugInfo` is
///   set.
/// </summary>
public interface IDelegateDebugInfo
{
  /// <summary>The lambda expression equivalent C# code</summary>
  string CSharpString { get; }
  /// <summary>The lambda expression object that was compiled to the delegate</summary>
  LambdaExpression Expression { get; }
  /// <summary>The lambda expression construction syntax C# code</summary>
  string ExpressionString { get; }
}