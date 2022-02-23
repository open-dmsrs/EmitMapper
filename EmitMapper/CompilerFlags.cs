using System;

namespace EmitMapper;

/// <summary>The options for the compiler</summary>
[Flags]
public enum CompilerFlags
{
  /// <summary>The default options: Invocation lambda is inlined, no debug info</summary>
  Default = 0,
  /// <summary>
  ///   Prevents the inlining of the lambda in the Invocation expression to optimize for the multiple same lambda
  ///   compiled once
  /// </summary>
  NoInvocationLambdaInlining = 1,
  /// <summary>Adds the Expression, ExpressionString, and CSharpString to the delegate closure for the debugging inspection</summary>
  EnableDelegateDebugInfo = 1 << 1,
  /// <summary>When the flag set then instead of the returning `null` the specific exception</summary>
  ThrowOnNotSupportedExpression = 1 << 2
}