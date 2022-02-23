using System;

namespace EmitMapper;

internal struct LiveCountArray<T>
{
  public int Count;
  public T[] Items;

  public LiveCountArray(T[] items)
  {
    Items = items;
    Count = items.Length;
  }

  public static T[] Expand(T[] items)
  {
    if (items.Length == 0)
      return new T[4];

    var count = items.Length;
    var newItems = new T[count << 1]; // count x 2
    Array.Copy(items, 0, newItems, 0, count);

    return newItems;
  }

  public void Pop()
  {
    --Count;
  }

  public ref T PushSlot()
  {
    if (++Count > Items.Length)
      Items = Expand(Items);

    return ref Items[Count - 1];
  }

  public void PushSlot(T item)
  {
    if (++Count > Items.Length)
      Items = Expand(Items);

    Items[Count - 1] = item;
  }
}