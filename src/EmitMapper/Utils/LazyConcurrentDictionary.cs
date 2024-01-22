using System.Collections.Concurrent;

namespace EmitMapper.Utils;

/// <summary>
///   The lazy concurrent dictionary.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class LazyConcurrentDictionary<TKey, TValue>
	where TKey : notnull
{
	private readonly ConcurrentDictionary<TKey, Lazy<TValue>> _inner;

	/// <summary>
	///   Initializes a new instance of the <see cref="LazyConcurrentDictionary&lt;TKey, TValue&gt;" /> class.
	/// </summary>
	public LazyConcurrentDictionary()
	  : this(Environment.ProcessorCount, 1024, null)
	{
	}

	/// <summary>
	///   Initializes a new instance of the <see cref="LazyConcurrentDictionary&lt;TKey, TValue&gt;" /> class.
	/// </summary>
	/// <param name="equatable">The equatable.</param>
	public LazyConcurrentDictionary(IEqualityComparer<TKey> equatable)
	  : this(Environment.ProcessorCount, 1024, equatable)
	{
	}

	/// <summary>
	///   Initializes a new instance of the <see cref="LazyConcurrentDictionary&lt;TKey, TValue&gt;" /> class.
	/// </summary>
	/// <param name="concurrencyLevel">The concurrency level.</param>
	/// <param name="capacity">The capacity.</param>
	public LazyConcurrentDictionary(int concurrencyLevel, int capacity)
	  : this(concurrencyLevel, capacity, null)
	{
	}

	/// <summary>
	///   Initializes a new instance of the <see cref="LazyConcurrentDictionary&lt;TKey, TValue&gt;" /> class.
	/// </summary>
	/// <param name="concurrencyLevel">The concurrency level.</param>
	/// <param name="capacity">The capacity.</param>
	/// <param name="equatable">The equatable.</param>
	public LazyConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey>? equatable)
	{
		_inner = new ConcurrentDictionary<TKey, Lazy<TValue>>(concurrencyLevel, capacity, equatable);
	}

	/// <summary>
	///   Adds the or update.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="addValueFactory">The add value factory.</param>
	/// <param name="updateValueFactory">The update value factory.</param>
	/// <returns>A TValue.</returns>
	public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
	{
		return _inner.AddOrUpdate(
		  key,
		  k => new Lazy<TValue>(() => addValueFactory(k)),
		  (k, currentValue) => new Lazy<TValue>(() => updateValueFactory(k, currentValue.Value))).Value;
	}

	/// <summary>
	///   Gets the or add.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="valueFactory">The value factory.</param>
	/// <returns>A TValue.</returns>
	public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
	{
		return _inner.GetOrAdd(key, k => new Lazy<TValue>(() => valueFactory(k))).Value;
	}

	// overload may not make sense to use when you want to avoid
	// the construction of the value when it isn't needed

	/// <summary>
	///   Try add.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	/// <returns>A bool.</returns>
	public bool TryAdd(TKey key, TValue value)
	{
		return _inner.TryAdd(key, new Lazy<TValue>(() => value));
	}

	/// <summary>
	///   Try add.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="valueFactory">The value factory.</param>
	/// <returns>A bool.</returns>
	public bool TryAdd(TKey key, Func<TKey, TValue> valueFactory)
	{
		return _inner.TryAdd(key, new Lazy<TValue>(() => valueFactory(key)));
	}

	/// <summary>
	///   Tries the get value.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	/// <returns>A bool.</returns>
	public bool TryGetValue(TKey key, out TValue value)
	{
		value = default;

		var result = _inner.TryGetValue(key, out var v);

		if (result)
		{
			value = v.Value;
		}

		return result;
	}

	/// <summary>
	///   Try remove.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	/// <returns>A bool.</returns>
	public bool TryRemove(TKey key, out TValue value)
	{
		value = default;

		if (_inner.TryRemove(key, out var v))
		{
			value = v.Value;

			return true;
		}

		return false;
	}

	/// <summary>
	///   Try update.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="updateValueFactory">The update value factory.</param>
	/// <returns>A bool.</returns>
	public bool TryUpdate(TKey key, Func<TKey, TValue, TValue> updateValueFactory)
	{
		if (!_inner.TryGetValue(key, out var existingValue))
		{
			return false;
		}

		return _inner.TryUpdate(key, new Lazy<TValue>(() => updateValueFactory(key, existingValue.Value)), existingValue);
	}
}