using System.ComponentModel;

namespace EmitMapper.Utils;

public abstract class ProxyBase
{
    protected void NotifyPropertyChanged(PropertyChangedEventHandler handler, string method)
    {
        handler?.Invoke(this, new PropertyChangedEventArgs(method));
    }
}