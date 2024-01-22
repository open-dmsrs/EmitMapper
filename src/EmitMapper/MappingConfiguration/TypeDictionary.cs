namespace EmitMapper.MappingConfiguration;

/// <summary>
///   The type dictionary.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class TypeDictionary<T>
  where T : class
{
	private readonly List<ListElement> elements = new();

	/// <summary>
	/// </summary>
	/// <param name="types">The types.</param>
	/// <param name="value">The value.</param>
	public void Add(Type[] types, T value)
	{
		var newElem = new ListElement(types, value);

		if (elements.Contains(newElem))
		{
			elements.Remove(newElem);
		}

		elements.Add(new ListElement(types, value));
	}

	/// <summary>
	///   Gets the value.
	/// </summary>
	/// <param name="types">The types.</param>
	/// <returns>A T.</returns>
	public T GetValue(Type[] types)
	{
		var elem = FindTypes(types);

		return elem?.Value;
	}

	/// <summary>
	///   Gets the value.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>A T.</returns>
	public T GetValue(Type type)
	{
		var elem = FindTypes(type);

		return elem?.Value;
	}

	/// <summary>
	///   Are the types in list.
	/// </summary>
	/// <param name="types">The types.</param>
	/// <returns>A bool.</returns>
	public bool IsTypesInList(Type[] types)
	{
		return FindTypes(types).HasValue;
	}

	/// <summary>
	///   Tos the string.
	/// </summary>
	/// <returns>A string.</returns>
	public override string ToString()
	{
		return elements.Select(e => e.Types.ToCsv("|") + (e.Value is null ? "|" : "|" + e.Value)).ToCsv("||");
	}

	/// <summary>
	///   Are the general type.
	/// </summary>
	/// <param name="generalType">The general type.</param>
	/// <param name="type">The type.</param>
	/// <returns>A bool.</returns>
	private static bool IsGeneralType(Type generalType, Type type)
	{
		if (generalType == type)
		{
			return true;
		}

		if (generalType.IsGenericTypeDefinition)
		{
			if (generalType.IsInterface)
			{
				return type.GetInterfacesCache().Concat(type.IsInterface ? new[] { type } : Type.EmptyTypes)
				  .Any(i => i.IsGenericType && i.GetGenericTypeDefinitionCache() == generalType);
			}

			return type.IsGenericType && (type.GetGenericTypeDefinitionCache() == generalType
										  || type.GetGenericTypeDefinitionCache().IsSubclassOf(generalType));
		}

		return generalType.IsAssignableFrom(type);
	}

	/// <summary>
	///   Finds the types.
	/// </summary>
	/// <param name="types">The types.</param>
	/// <returns>A ListElement? .</returns>
	private ListElement? FindTypes(Type[] types)
	{
		foreach (var element in elements)
		{
			var isAssignable = true;

			for (int i = 0, j = 0; i < element.Types.Length; ++i)
			{
				if (i < types.Length)
				{
					j = i;
				}

				if (IsGeneralType(element.Types[i], types[j]))
				{
					continue;
				}

				isAssignable = false;

				break;
			}

			if (isAssignable)
			{
				return element;
			}
		}

		return null;
	}

	/// <summary>
	///   Finds the types.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>A ListElement? .</returns>
	private ListElement? FindTypes(Type type)
	{
		foreach (var element in elements)
		{
			var isAssignable = true;

			foreach (var t in element.Types)
			{
				if (IsGeneralType(t, type))
				{
					continue;
				}

				isAssignable = false;

				break;
			}

			if (isAssignable)
			{
				return element;
			}
		}

		return null;
	}

	private readonly struct ListElement : IEquatable<ListElement>
	{
		/// <summary>
		/// Types
		/// </summary>
		public readonly Type[] Types;

		/// <summary>
		/// Value
		/// </summary>
		public readonly T Value;

		/// <summary>
		/// Initializes a new instance of the <see cref="ListElement"/> struct.
		/// Initializes a new instance of the <see cref="ListElement"/> class.
		/// </summary>
		/// <param name="types">The types.</param>
		/// <param name="value">The value.</param>
		public ListElement(Type[] types, T value)
		{
			Types = types;
			Value = value;
		}

		/// <summary>
		/// Equals the.
		/// </summary>
		/// <param name="other">The other.</param>
		/// <returns>A bool.</returns>
		public bool Equals(ListElement other)
		{
			return !Types.Where((t, i) => t != other.Types[i]).Any();
		}

		/// <summary>
		/// Equals the.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns>A bool.</returns>
		public override bool Equals(object obj)
		{
			return Equals(obj);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns>An int.</returns>
		public override int GetHashCode()
		{
			return HashCode.Combine(Types);
		}
	}
}