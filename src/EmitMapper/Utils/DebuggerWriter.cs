using System.Text;

namespace EmitMapper;

/// <summary>
///   Implements a <see cref="TextWriter" /> for writing information to the debugger log.
/// </summary>
/// <seealso cref="Debugger.Log" />
public class DebuggerWriter : TextWriter
{
  private static UnicodeEncoding _encoding;

  private bool _isOpen;

  /// <summary>
  ///   Initializes a new instance of the <see cref="DebuggerWriter" /> class.
  /// </summary>
  public DebuggerWriter()
    : this(0, Debugger.DefaultCategory)
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="DebuggerWriter" /> class with the specified level and category.
  /// </summary>
  /// <param name="level">A description of the importance of the messages.</param>
  /// <param name="category">The category of the messages.</param>
  public DebuggerWriter(int level, string category)
    : this(level, category, CultureInfo.CurrentCulture)
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="DebuggerWriter" /> class with the specified level, category and format
  ///   provider.
  /// </summary>
  /// <param name="level">A description of the importance of the messages.</param>
  /// <param name="category">The category of the messages.</param>
  /// <param name="formatProvider">An <see cref="IFormatProvider" /> object that controls formatting.</param>
  public DebuggerWriter(int level, string category, IFormatProvider formatProvider)
    : base(formatProvider)
  {
    Level = level;
    Category = category;
    _isOpen = true;
  }

  /// <summary>
  ///   Gets the category.
  /// </summary>
  public string Category { get; }

  /// <summary>
  ///   Gets the encoding.
  /// </summary>
  public override Encoding Encoding => _encoding ??= new UnicodeEncoding(false, false);

  /// <summary>
  ///   Gets the level.
  /// </summary>
  public int Level { get; }

  /// <summary>
  /// </summary>
  /// <param name="value">The value.</param>
  /// <exception cref="ObjectDisposedException"></exception>
  public override void Write(char value)
  {
    if (!_isOpen) throw new ObjectDisposedException(null);

    Debugger.Log(Level, Category, value.ToString());
  }

  /// <summary>
  /// </summary>
  /// <param name="value">The value.</param>
  /// <exception cref="ObjectDisposedException"></exception>
  public override void Write(string value)
  {
    if (!_isOpen) throw new ObjectDisposedException(null);

    if (value != null) Debugger.Log(Level, Category, value);
  }

  /// <summary>
  /// </summary>
  /// <param name="buffer">The buffer.</param>
  /// <param name="index">The index.</param>
  /// <param name="count">The count.</param>
  /// <exception cref="ObjectDisposedException"></exception>
  public override void Write(char[] buffer, int index, int count)
  {
    if (!_isOpen) throw new ObjectDisposedException(null);

    if (index < 0 || count < 0 || buffer.Length - index < count)
      base.Write(buffer, index, count); // delegate throw exception to base class

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