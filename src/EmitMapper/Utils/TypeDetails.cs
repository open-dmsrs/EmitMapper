namespace EmitMapper.Utils;

/// <summary>
///   Contains cached reflection information for easy retrieval
/// </summary>
[DebuggerDisplay("{" + nameof(Type) + "}")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class TypeDetails
{
	private ConstructorParameters[] _constructors;

	private Dictionary<string, MemberInfo> _nameToMember;

	private MemberInfo[] _readAccessors;

	private MemberInfo[] _writeAccessors;

	/// <summary>
	///   Initializes a new instance of the <see cref="TypeDetails" /> class.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="config">The config.</param>
	public TypeDetails(Type type, ProfileMap config)
	{
		Type = type;
		Config = config;
	}

	/// <summary>
	///   Gets the config.
	/// </summary>
	public ProfileMap Config { get; }

	/// <summary>
	///   Gets the constructors.
	/// </summary>
	public ConstructorParameters[] Constructors => _constructors ??= GetConstructors();

	/// <summary>
	///   Gets the read accessors.
	/// </summary>
	public MemberInfo[] ReadAccessors => _readAccessors ??= BuildReadAccessors();

	/// <summary>
	///   Gets the type.
	/// </summary>
	public Type Type { get; }

	/// <summary>
	///   Gets the write accessors.
	/// </summary>
	public MemberInfo[] WriteAccessors => _writeAccessors ??= BuildWriteAccessors();

	/// <summary>
	///   Gets the constructors.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="profileMap">The profile map.</param>
	/// <returns><![CDATA[IEnumerable<ConstructorParameters>]]></returns>
	public static IEnumerable<ConstructorParameters> GetConstructors(Type type, ProfileMap profileMap)
	{
		return type.GetDeclaredConstructors().Where(profileMap.ShouldUseConstructor)
		  .Select(c => new ConstructorParameters(c));
	}

	/// <summary>
	///   Possibles the names.
	/// </summary>
	/// <param name="memberName">The member name.</param>
	/// <param name="prefixes">The prefixes.</param>
	/// <param name="postfixes">The postfixes.</param>
	/// <returns><![CDATA[IEnumerable<string>]]></returns>
	public static IEnumerable<string> PossibleNames(string memberName, List<string> prefixes, List<string> postfixes)
	{
		foreach (var prefix in prefixes)
		{
			if (!memberName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}

			var withoutPrefix = memberName.Substring(prefix.Length);

			yield return withoutPrefix;

			foreach (var s in PostFixes(postfixes, withoutPrefix))
			{
				yield return s;
			}
		}

		foreach (var s in PostFixes(postfixes, memberName))
		{
			yield return s;
		}
	}

	/// <summary>
	///   Gets the member.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <returns>A MemberInfo.</returns>
	public MemberInfo GetMember(string name)
	{
		_nameToMember ??= PossibleNames();

		return _nameToMember.GetOrDefault(name);
	}

	/// <summary>
	///   Field readable.
	/// </summary>
	/// <param name="fieldInfo">The field info.</param>
	/// <returns>A bool.</returns>
	private static bool FieldReadable(FieldInfo fieldInfo)
	{
		return true;
	}

	/// <summary>
	///   Field writable.
	/// </summary>
	/// <param name="fieldInfo">The field info.</param>
	/// <returns>A bool.</returns>
	private static bool FieldWritable(FieldInfo fieldInfo)
	{
		return !fieldInfo.IsInitOnly;
	}

	/// <summary>
	///   Posts the fixes.
	/// </summary>
	/// <param name="postfixes">The postfixes.</param>
	/// <param name="name">The name.</param>
	/// <returns><![CDATA[IEnumerable<string>]]></returns>
	private static IEnumerable<string> PostFixes(List<string> postfixes, string name)
	{
		foreach (var postfix in postfixes)
		{
			if (!name.EndsWith(postfix, StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}

			yield return name.Remove(name.Length - postfix.Length);
		}
	}

	/// <summary>
	///   Property readable.
	/// </summary>
	/// <param name="propertyInfo">The property info.</param>
	/// <returns>A bool.</returns>
	private static bool PropertyReadable(PropertyInfo propertyInfo)
	{
		return propertyInfo.CanRead;
	}

	/// <summary>
	///   Property writable.
	/// </summary>
	/// <param name="propertyInfo">The property info.</param>
	/// <returns>A bool.</returns>
	private static bool PropertyWritable(PropertyInfo propertyInfo)
	{
		return propertyInfo.CanWrite || propertyInfo.PropertyType.IsCollection();
	}

	/// <summary>
	///   Adds the methods.
	/// </summary>
	/// <param name="accessors">The accessors.</param>
	/// <returns><![CDATA[IEnumerable<MemberInfo>]]></returns>
	private IEnumerable<MemberInfo> AddMethods(IEnumerable<MemberInfo> accessors)
	{
		var publicNoArgMethods = GetPublicNoArgMethods();

		var publicNoArgExtensionMethods =
		  GetPublicNoArgExtensionMethods(Config.SourceExtensionMethods.Where(Config.ShouldMapMethod));

		return accessors.Concat(publicNoArgMethods).Concat(publicNoArgExtensionMethods);
	}

	/// <summary>
	///   Builds the read accessors.
	/// </summary>
	/// <returns>An array of MemberInfos</returns>
	private MemberInfo[] BuildReadAccessors()
	{
		// Multiple types may define the same property (e.g. the class and multiple interfaces) - filter this to one of those properties
		IEnumerable<MemberInfo> members = GetProperties(PropertyReadable)
		  .GroupBy(x => x.Name) // group properties of the same name together
		  .Select(x => x.First());

		if (Config.FieldMappingEnabled)
		{
			members = members.Concat(GetFields(FieldReadable));
		}

		return members.ToArray();
	}

	/// <summary>
	///   Builds the write accessors.
	/// </summary>
	/// <returns>An array of MemberInfos</returns>
	private MemberInfo[] BuildWriteAccessors()
	{
		// Multiple types may define the same property (e.g. the class and multiple interfaces) - filter this to one of those properties
		IEnumerable<MemberInfo> members = GetProperties(PropertyWritable)
		  .GroupBy(x => x.Name) // group properties of the same name together
		  .Select(
			x => x.FirstOrDefault(y => y.CanWrite && y.CanRead)
				 ?? x.First()); // favor the first property that can both read & write - otherwise pick the first one

		if (Config.FieldMappingEnabled)
		{
			members = members.Concat(GetFields(FieldWritable));
		}

		return members.ToArray();
	}

	/// <summary>
	///   Checks the pre postfixes.
	/// </summary>
	/// <param name="nameToMember">The name to member.</param>
	/// <param name="member">The member.</param>
	private void CheckPrePostfixes(IDictionary<string, MemberInfo> nameToMember, MemberInfo member)
	{
		foreach (var memberName in PossibleNames(member.Name, Config.Prefixes, Config.Postfixes))
		{
			if (!nameToMember.ContainsKey(memberName))
			{
				nameToMember.Add(memberName, member);
			}
		}
	}

	/// <summary>
	///   Gets the constructors.
	/// </summary>
	/// <returns>An array of ConstructorParameters</returns>
	private ConstructorParameters[] GetConstructors()
	{
		return GetConstructors(Type, Config).Where(c => c.ParametersCount > 0).OrderByDescending(c => c.ParametersCount)
		  .ToArray();
	}

	/// <summary>
	///   Gets the fields.
	/// </summary>
	/// <param name="fieldAvailableFor">The field available for.</param>
	/// <returns><![CDATA[IEnumerable<MemberInfo>]]></returns>
	private IEnumerable<MemberInfo> GetFields(Func<FieldInfo, bool> fieldAvailableFor)
	{
		return GetTypeInheritance().SelectMany(
		  type => type.GetFields(TypeExtensions.InstanceFlags)
			.Where(field => fieldAvailableFor(field) && Config.ShouldMapField(field)));
	}

	/// <summary>
	///   Gets the properties.
	/// </summary>
	/// <param name="propertyAvailableFor">The property available for.</param>
	/// <returns><![CDATA[IEnumerable<PropertyInfo>]]></returns>
	private IEnumerable<PropertyInfo> GetProperties(Func<PropertyInfo, bool> propertyAvailableFor)
	{
		return GetTypeInheritance().SelectMany(
		  type => type.GetProperties(TypeExtensions.InstanceFlags).Where(
			property => propertyAvailableFor(property) && Config.ShouldMapProperty(property)));
	}

	/// <summary>
	///   Gets the public no arg extension methods.
	/// </summary>
	/// <param name="sourceExtensionMethodSearch">The source extension method search.</param>
	/// <returns><![CDATA[IEnumerable<MethodInfo>]]></returns>
	private IEnumerable<MethodInfo> GetPublicNoArgExtensionMethods(IEnumerable<MethodInfo> sourceExtensionMethodSearch)
	{
		var explicitExtensionMethods =
		  sourceExtensionMethodSearch.Where(method => method.GetParameters()[0].ParameterType.IsAssignableFrom(Type));

		var genericInterfaces = Type.GetInterfacesCache().Where(t => t.IsGenericType);
		if (Type.IsInterface && Type.IsGenericType)
		{
			genericInterfaces = genericInterfaces.Union(new[] { Type });
		}

		return explicitExtensionMethods.Union(
		  from genericInterface in genericInterfaces
		  let genericInterfaceArguments = genericInterface.GenericTypeArguments
		  let matchedMethods =
			(from extensionMethod in sourceExtensionMethodSearch
			 where !extensionMethod.IsGenericMethodDefinition
			 select extensionMethod)
			.Concat(
			  from extensionMethod in sourceExtensionMethodSearch
			  where extensionMethod.IsGenericMethodDefinition
					&& extensionMethod.GetGenericArguments().Length == genericInterfaceArguments.Length
			  select extensionMethod.MakeGenericMethod(genericInterfaceArguments))
		  from methodMatch in matchedMethods
		  where methodMatch.GetParameters()[0].ParameterType.IsAssignableFrom(genericInterface)
		  select methodMatch);
	}

	/// <summary>
	///   Gets the public no arg methods.
	/// </summary>
	/// <returns><![CDATA[IEnumerable<MethodInfo>]]></returns>
	private IEnumerable<MethodInfo> GetPublicNoArgMethods()
	{
		return Type.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(
		  m => m.DeclaringType != Metadata<object>.Type && m.ReturnType != Metadata.Void && Config.ShouldMapMethod(m)
			   && m.GetParameters().Length == 0);
	}

	/// <summary>
	///   Gets the type inheritance.
	/// </summary>
	/// <returns><![CDATA[IEnumerable<Type>]]></returns>
	private IEnumerable<Type> GetTypeInheritance()
	{
		return Type.IsInterface
		  ? new[] { Type }.Concat(Type.GetInterfacesCache())
		  : Type.GetTypeInheritance();
	}

	/// <summary>
	///   Possibles the names.
	/// </summary>
	/// <returns><![CDATA[Dictionary<string, MemberInfo>]]></returns>
	private Dictionary<string, MemberInfo> PossibleNames()
	{
		var nameToMember = new Dictionary<string, MemberInfo>(ReadAccessors.Length, StringComparer.OrdinalIgnoreCase);
		IEnumerable<MemberInfo> accessors = ReadAccessors;
		if (Config.MethodMappingEnabled)
		{
			accessors = AddMethods(accessors);
		}

		foreach (var member in accessors)
		{
			if (!nameToMember.ContainsKey(member.Name))
			{
				nameToMember.Add(member.Name, member);
			}

			if (Config.Postfixes.Count == 0 && Config.Prefixes.Count == 0)
			{
				continue;
			}

			CheckPrePostfixes(nameToMember, member);
		}

		return nameToMember;
	}
}