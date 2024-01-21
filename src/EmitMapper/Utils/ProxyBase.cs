namespace EmitMapper.Utils;

/// <summary>
///   The proxy base.
/// </summary>
public abstract class ProxyBase
{
	/// <summary>
	///   Notifies the property changed.
	/// </summary>
	/// <param name="handler">The handler.</param>
	/// <param name="method">The method.</param>
	public void NotifyPropertyChanged(PropertyChangedEventHandler? handler, string method)
	{
		handler?.Invoke(this, new PropertyChangedEventArgs(method));
	}
}