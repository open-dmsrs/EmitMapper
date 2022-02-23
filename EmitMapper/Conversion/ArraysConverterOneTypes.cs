using System.Collections.Generic;
using System.Linq;

namespace EmitMapper.Conversion;

internal class ArraysConverterOneTypes<T>
{
  public T[] Convert(ICollection<T> from, object state)
  {
    return from?.ToArray();
  }
}