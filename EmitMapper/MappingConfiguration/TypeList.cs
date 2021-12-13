namespace EmitMapper.MappingConfiguration;

using System;
using System.Collections.Generic;
using System.Linq;

using EmitMapper.Utils;

internal class TypeDictionary<T>
    where T : class
{
    private readonly List<ListElement> _elements = new();

    public override string ToString()
    {
        return this._elements.Select(e => e.Types.ToCsv("|") + (e.Value == null ? "|" : "|" + e.Value)).ToCsv("||");
    }

    public bool IsTypesInList(Type[] types)
    {
        return this.FindTypes(types) != null;
    }

    public T GetValue(Type[] types)
    {
        var elem = this.FindTypes(types);
        return elem == null ? null : elem.Value;
    }

    public void Add(Type[] types, T value)
    {
        var newElem = new ListElement(types, value);
        if (this._elements.Contains(newElem))
            this._elements.Remove(newElem);

        this._elements.Add(new ListElement(types, value));
    }

    private ListElement FindTypes(Type[] types)
    {
        foreach (var element in this._elements)
        {
            var isAssignable = true;
            for (int i = 0, j = 0; i < element.Types.Length; ++i)
            {
                if (i < types.Length)
                    j = i;

                if (!IsGeneralType(element.Types[i], types[j]))
                {
                    isAssignable = false;
                    break;
                }
            }

            if (isAssignable)
                return element;
        }

        return null;
    }

    private static bool IsGeneralType(Type generalType, Type type)
    {
        if (generalType == type)
            return true;
        if (generalType.IsGenericTypeDefinition)
        {
            if (generalType.IsInterface)
                return (type.IsInterface ? new[] { type } : new Type[0]).Concat(type.GetInterfaces()).Any(
                    i => i.IsGenericType && i.GetGenericTypeDefinition() == generalType);
            return type.IsGenericType && (type.GetGenericTypeDefinition() == generalType
                                          || type.GetGenericTypeDefinition().IsSubclassOf(generalType));
        }

        return generalType.IsAssignableFrom(type);
    }

    private class ListElement
    {
        public readonly Type[] Types;

        public readonly T Value;

        public ListElement(Type[] types, T value)
        {
            this.Types = types;
            this.Value = value;
        }

        public override int GetHashCode()
        {
            return this.Types.Sum(t => t.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            var rhs = (ListElement)obj;
            for (var i = 0; i < this.Types.Length; ++i)
                if (this.Types[i] != rhs.Types[i])
                    return false;
            return true;
        }
    }
}