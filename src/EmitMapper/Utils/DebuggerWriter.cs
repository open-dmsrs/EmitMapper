using System.Text;

namespace EmitMapper;

/// <summary>
/// Implements a <see cref="TextWriter"/> for writing information to the debugger log.
/// </summary>
public class DebuggerWriter : TextWriter
{
	private static UnicodeEncoding? _encoding;

	private bool _isOpen;

	/// <summary>
	/// Initializes a new instance of the <see cref="DebuggerWriter"/> class.
	/// </summary>
	public DebuggerWriter()
	  : this(0, Debugger.DefaultCategory)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DebuggerWriter"/> class with the specified level
	/// and category.
	/// </summary>
	/// <param name="level">A description of the importance of the messages.</param>
	/// <param name="category">The category of the messages.</param>
	public DebuggerWriter(int level, string? category)
	  : this(level, category, CultureInfo.CurrentCulture)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DebuggerWriter"/> class with the specified level,
	/// category and format provider.
	/// </summary>
	/// <param name="level">A description of the importance of the messages.</param>
	/// <param name="category">The category of the messages.</param>
	/// <param name="formatProvider">An <see cref="IFormatProvider"/> object that controls formatting.</param>
	public DebuggerWriter(int level, string? category, IFormatProvider formatProvider)
	  : base(formatProvider)
	{
		Level = level;
		Category = category;
		_isOpen = true;
	}

	/// <summary>
	/// Gets the category.
	/// </summary>
	public string? Category { get; }

	/// <summary>
	/// Gets the encoding.
	/// </summary>
	public override Encoding Encoding => _encoding ??= new UnicodeEncoding(false, false);

	/// <summary>
	/// Gets the level.
	/// </summary>
	public int Level { get; }

	/// <inheritdoc/>
	public override void Write(char value)
	{
		ObjectDisposedException.ThrowIf(!_isOpen, Metadata<DebuggerWriter>.Type);

		Debugger.Log(Level, Category, value.ToString());
	}

	/// <inheritdoc/>
	public override void Write(string? value)
	{
		ObjectDisposedException.ThrowIf(!_isOpen, Metadata<DebuggerWriter>.Type);

		if (value is not null)
		{
			Debugger.Log(Level, Category, value);
		}
	}

	/// <inheritdoc/>
	public override void Write(char[] buffer, int index, int count)
	{
		ObjectDisposedException.ThrowIf(!_isOpen, Metadata<DebuggerWriter>.Type);

		if (index < 0 || count < 0 || buffer.Length - index < count)
		{
			base.Write(buffer, index, count); // delegate throw exception to base class
		}

		Debugger.Log(Level, Category, new string(buffer, index, count));
	}

	/// <summary>
	/// </summary>
	/// <param name="disposing">If true, disposing.</param>
	protected override void Dispose(bool disposing)
	{
		_isOpen = false;
		base.Dispose(disposing);
	}
}