using System;

namespace EmitMapper.AST;

/// <summary>
///   The i l compilation exception.
/// </summary>
internal class ILCompilationException : Exception
{
  /// <summary>
  ///   Initializes a new instance of the <see cref="ILCompilationException" /> class.
  /// </summary>
  /// <param name="message">The message.</param>
  public ILCompilationException(string message)
    : base(message)
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="ILCompilationException" /> class.
  /// </summary>
  /// <param name="message">The message.</param>
  /// <param name="p">The p.</param>
  public ILCompilationException(string message, params object[] p)
    : base(string.Format(message, p))
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="ILCompilationException" /> class.
  /// </summary>
  public ILCompilationException()
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="ILCompilationException" /> class.
  /// </summary>
  /// <param name="message">The message.</param>
  /// <param name="innerException">The inner exception.</param>
  public ILCompilationException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}