namespace EmitMapper.AST;

/// <summary>
///   The i l compilation exception.
/// </summary>
internal class IlCompilationException : Exception
{
	/// <summary>
	///   Initializes a new instance of the <see cref="IlCompilationException" /> class.
	/// </summary>
	/// <param name="message">The message.</param>
	public IlCompilationException(string message)
	  : base(message)
	{
	}

	/// <summary>
	///   Initializes a new instance of the <see cref="IlCompilationException" /> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="p">The p.</param>
	public IlCompilationException(string message, params object[] p)
	  : base(string.Format(CultureInfo.InvariantCulture, message, p))
	{
	}

	/// <summary>
	///   Initializes a new instance of the <see cref="IlCompilationException" /> class.
	/// </summary>
	public IlCompilationException()
	{
	}

	/// <summary>
	///   Initializes a new instance of the <see cref="IlCompilationException" /> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="innerException">The inner exception.</param>
	public IlCompilationException(string message, Exception innerException)
	  : base(message, innerException)
	{
	}
}