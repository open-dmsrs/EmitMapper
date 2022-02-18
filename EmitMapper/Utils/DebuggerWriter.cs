using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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

  public string Category { get; }

  public override Encoding Encoding => _encoding ??= new UnicodeEncoding(false, false);

  public int Level { get; }

  public override void Write(char value)
  {
    if (!_isOpen) throw new ObjectDisposedException(null);
    Debugger.Log(Level, Category, value.ToString());
  }

  public override void Write(string value)
  {
    if (!_isOpen) throw new ObjectDisposedException(null);
    if (value != null) Debugger.Log(Level, Category, value);
  }

  public override void Write(char[] buffer, int index, int count)
  {
    if (!_isOpen) throw new ObjectDisposedException(null);
    if (index < 0 || count < 0 || buffer.Length - index < count)
      base.Write(buffer, index, count); // delegate throw exception to base class
    Debugger.Log(Level, Category, new string(buffer, index, count));
  }

  protected override void Dispose(bool disposing)
  {
    _isOpen = false;
    base.Dispose(disposing);
  }
}