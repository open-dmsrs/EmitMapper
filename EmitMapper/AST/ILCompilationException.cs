using System;

namespace EmitMapper.AST;

internal class ILCompilationException : Exception
{
  public ILCompilationException(string message)
    : base(message)
  {
  }

  public ILCompilationException(string message, params object[] p)
    : base(string.Format(message, p))
  {
  }

  public ILCompilationException()
  {
  }

  public ILCompilationException(string message, Exception innerException) : base(message, innerException)
  {
  }
}