using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace EmitMapper;

/// <summary>
/// Implements a <see cref="TextWriter"/> for writing information to the debugger log.
/// </summary>
/// <seealso cref="Debugger.Log"/>
public class DebuggerWriter : TextWriter
{
    private bool _isOpen;
    private static UnicodeEncoding _encoding;
    private readonly int _level;
    private readonly string _category;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebuggerWriter"/> class.
    /// </summary>
    public DebuggerWriter()
        : this(0, Debugger.DefaultCategory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebuggerWriter"/> class with the specified level and category.
    /// </summary>
    /// <param name="level">A description of the importance of the messages.</param>
    /// <param name="category">The category of the messages.</param>
    public DebuggerWriter(int level, string category)
        : this(level, category, CultureInfo.CurrentCulture)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebuggerWriter"/> class with the specified level, category and format provider.
    /// </summary>
    /// <param name="level">A description of the importance of the messages.</param>
    /// <param name="category">The category of the messages.</param>
    /// <param name="formatProvider">An <see cref="IFormatProvider"/> object that controls formatting.</param>
    public DebuggerWriter(int level, string category, IFormatProvider formatProvider)
        : base(formatProvider)
    {
        this._level = level;
        this._category = category;
        this._isOpen = true;
    }

    protected override void Dispose(bool disposing)
    {
        _isOpen = false;
        base.Dispose(disposing);
    }

    public override void Write(char value)
    {
        if (!_isOpen)
        {
            throw new ObjectDisposedException(null);
        }
        Debugger.Log(_level, _category, value.ToString());
    }

    public override void Write(string value)
    {
        if (!_isOpen)
        {
            throw new ObjectDisposedException(null);
        }
        if (value != null)
        {
            Debugger.Log(_level, _category, value);
        }
    }

    public override void Write(char[] buffer, int index, int count)
    {
        if (!_isOpen)
        {
            throw new ObjectDisposedException(null);
        }
        if (buffer == null || index < 0 || count < 0 || buffer.Length - index < count)
        {
            base.Write(buffer, index, count); // delegate throw exception to base class
        }
        Debugger.Log(_level, _category, new string(buffer, index, count));
    }

    public override Encoding Encoding
    {
        get
        {
            if (_encoding == null)
            {
                _encoding = new UnicodeEncoding(false, false);
            }
            return _encoding;
        }
    }

    public int Level
    {
        get { return _level; }
    }

    public string Category
    {
        get { return _category; }
    }
}