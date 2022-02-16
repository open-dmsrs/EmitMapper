namespace EmitMapper.Utils;

using System.ComponentModel;

public abstract class ProxyBase
{
  public void NotifyPropertyChanged(PropertyChangedEventHandler handler, string method)
  {
    handler?.Invoke(this, new PropertyChangedEventArgs(method));
  }
}