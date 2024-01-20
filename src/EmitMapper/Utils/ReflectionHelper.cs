using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace EmitMapper.Utils;

/// <summary>
///   The reflection helper.
/// </summary>
public static class ReflectionHelper
{
	private const BindingFlags BindingFlagsInstancePublic = BindingFlags.Instance | BindingFlags.Public;

	private static readonly LazyConcurrentDictionary<Type, ReadOnlyCollection<MemberInfo>> AllMemberInfo = new();

	private static readonly LazyConcurrentDictionary<Type, Type> GenericTypeDefinitionCache = new();

	private static readonly LazyConcurrentDictionary<Type, Type[]> Interfaces = new();

	private static readonly LazyConcurrentDictionary<Type, bool> IsNullableCache = new();

	private static readonly LazyConcurrentDictionary<MemberInfo, Type> MemberInfoReturnTypes = new();

	private static readonly LazyConcurrentDictionary<string, MethodInfo> MethodsCahce = new();

	private static readonly LazyConcurrentDictionary<Type, Type> UnderlyingTypes = new();

	/// <summary>
	///   Can be set.
	/// </summary>
	/// <param name="member">The member.</param>
	/// <returns>A bool.</returns>
	public static bool CanBeSet(this MemberInfo member)
	{
		return member is PropertyInfo property ? property.CanWrite : !((FieldInfo)member).IsInitOnly;
	}

	/// <summary>
	///   Converts the value.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="fieldType">The field type.</param>
	/// <param name="destinationType">The destination type.</param>
	/// <exception cref="NotImplementedException"></exception>
	/// <returns>An object.</returns>
	public static object ConvertValue(object value, Type fieldType, Type destinationType)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	///   Finds the property.
	/// </summary>
	/// <param name="lambdaExpression">The lambda expression.</param>
	/// <exception cref="ArgumentException"></exception>
	/// <returns>A MemberInfo.</returns>
	public static MemberInfo FindProperty(LambdaExpression lambdaExpression)
	{
		var expressionToCheck = lambdaExpression.Body;

		while (true)
		{
			switch (expressionToCheck)
			{
				case MemberExpression
				{
					Member: var member, Expression: { NodeType: ExpressionType.Parameter or ExpressionType.Convert }
				}:
					return member;
				case UnaryExpression { Operand: var operand }:
					expressionToCheck = operand;

					break;
				default:
					throw new ArgumentException(
					  $"Expression '{lambdaExpression}' must resolve to top-level member and not any child object's properties. You can use ForPath, a custom resolver on the child type or the AfterMap option instead.",
					  nameof(lambdaExpression));
			}
		}
	}

	/// <summary>
	///   Gets the common members.
	/// </summary>
	/// <param name="first">The first.</param>
	/// <param name="second">The second.</param>
	/// <param name="matcher">The matcher.</param>
	/// <returns>An array of MatchedMembers</returns>
	public static MatchedMember[] GetCommonMembers(Type first, Type second, Func<string, string, bool> matcher)
	{
		matcher ??= (f, s) => f == s;
		var firstMembers = GetPublicFieldsAndProperties(first);
		var secondMembers = GetPublicFieldsAndProperties(second);
		var result = new List<MatchedMember>();

		foreach (var f in firstMembers)
		{
			var s = secondMembers.FirstOrDefault(sm => matcher(f.Name, sm.Name));

			if (s != null)
			{
				result.Add(new MatchedMember(f, s));
			}
		}

		return result.ToArray();
	}

	/// <summary>
	///   Gets the data member definition.
	/// </summary>
	/// <param name="destinationMember">The destination member.</param>
	/// <exception cref="NotImplementedException"></exception>
	/// <returns><![CDATA[IEnumerable<Tuple<string, Type>>]]></returns>
	public static IEnumerable<Tuple<string, Type>> GetDataMemberDefinition(MemberInfo destinationMember)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	///   Gets the default value.
	/// </summary>
	/// <param name="parameter">The parameter.</param>
	/// <returns>An Expression.</returns>
	public static Expression GetDefaultValue(this ParameterInfo parameter)
	{
		return parameter is { DefaultValue: null, ParameterType: { IsValueType: true } type }
		  ? Default(type)
		  : Constant(parameter.DefaultValue);
	}

	/// <summary>
	///   Gets the element type.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>A Type.</returns>
	public static Type? GetElementType(Type type)
	{
		return type.IsArray ? type.GetElementType() : GetEnumerableElementType(type);
	}

	/// <summary>
	///   Gets the enumerable element type.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>A Type.</returns>
	public static Type GetEnumerableElementType(Type type)
	{
		return type.GetIEnumerableType()?.GenericTypeArguments[0] ?? Metadata<object>.Type;
	}

	/// <summary>
	///   Gets the generic type definition cache.
	/// </summary>
	/// <param name="t">The t.</param>
	/// <returns>A Type.</returns>
	public static Type GetGenericTypeDefinitionCache(this Type t)
	{
		return GenericTypeDefinitionCache.GetOrAdd(t, type => type.IsGenericType ? type.GetGenericTypeDefinition() : null);
	}

	/// <summary>
	///   Gets the interfaces cache.
	/// </summary>
	/// <param name="t">The t.</param>
	/// <returns>An array of Types</returns>
	public static Type[] GetInterfacesCache(this Type t)
	{
		return Interfaces.GetOrAdd(t, type => type.GetInterfaces());
	}

	/// <summary>
	///   Gets the member path.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="fullMemberName">The full member name.</param>
	/// <returns>An array of MemberInfos</returns>
	public static MemberInfo[] GetMemberPath(Type type, string fullMemberName)
	{
		var memberNames = fullMemberName.Split('.');
		var members = new MemberInfo[memberNames.Length];
		var previousType = type;

		for (var index = 0; index < memberNames.Length; index++)
		{
			var currentType = GetCurrentType(previousType);
			var memberName = memberNames[index];
			var property = currentType.GetInheritedProperty(memberName);

			if (property != null)
			{
				previousType = property.PropertyType;
				members[index] = property;
			}
			else if (currentType.GetInheritedField(memberName) is FieldInfo field)
			{
				previousType = field.FieldType;
				members[index] = field;
			}
			else
			{
				var method = currentType.GetInheritedMethod(memberName);
				previousType = method.ReturnType;
				members[index] = method;
			}
		}

		return members;

		static Type GetCurrentType(Type type)
		{
			return type.IsGenericType && type.IsCollection() ? type.GenericTypeArguments[0] : type;
		}
	}

	/// <summary>
	///   Gets the member return type.
	/// </summary>
	/// <param name="member">The member.</param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <returns>A Type.</returns>
	public static Type GetMemberReturnType(MemberInfo member)
	{
		return MemberInfoReturnTypes.GetOrAdd(
		  member,
		  m => m switch
		  {
			  PropertyInfo property => property.PropertyType,
			  MethodInfo method => method.ReturnType,
			  FieldInfo field => field.FieldType,
			  null => throw new ArgumentNullException(nameof(member)),
			  _ => throw new ArgumentOutOfRangeException(nameof(member))
		  });
	}

	/// <summary>
	///   Gets the member type.
	/// </summary>
	/// <param name="member">The member.</param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <returns>A Type.</returns>
	public static Type GetMemberType(this MemberInfo member)
	{
		return member switch
		{
			PropertyInfo property => property.PropertyType,
			MethodInfo method => method.ReturnType,
			FieldInfo field => field.FieldType,
			null => throw new ArgumentNullException(nameof(member)),
			_ => throw new ArgumentOutOfRangeException(nameof(member))
		};
	}

	/// <summary>
	///   Gets the member value.
	/// </summary>
	/// <param name="propertyOrField">The property or field.</param>
	/// <param name="target">The target.</param>
	/// <returns>An object.</returns>
	public static object GetMemberValue(this MemberInfo propertyOrField, object target)
	{
		return propertyOrField switch
		{
			PropertyInfo property => property.GetValue(target, null),
			FieldInfo field => field.GetValue(target),
			_ => throw Expected(propertyOrField)
		};
	}

	/// <summary>
	///   Bug: cached a method info on a type, just one method on one type. there is bug if get two methods from a same type
	/// </summary>
	/// <param name="t"></param>
	/// <param name="name"></param>
	/// <returns></returns>
	public static MethodInfo GetMethodCache(this Type t, string name)
	{
		var k = $"{t.FullName}::{name}";

		return MethodsCahce.GetOrAdd(k, _ => t.GetMethod(name));
	}

	/// <summary>
	///   Fixed: Get Full hierarchy with all parent interfaces members.
	/// </summary>
	public static IEnumerable<MemberInfo> GetPublicFieldsAndProperties(Type t)
	{
		return AllMemberInfo.GetOrAdd(
		  t,
		  type => type.GetMembers(BindingFlags.Instance | BindingFlags.Public)
			.Where(mi => mi.MemberType == MemberTypes.Property || mi.MemberType == MemberTypes.Field)
			.Concat(type.GetInterfacesCache().SelectMany(GetPublicFieldsAndProperties)).ToList().AsReadOnly());
	}

	/// <summary>
	///   获取源的数据类型
	/// </summary>
	/// <param name="t"></param>
	/// <returns></returns>
	public static IEnumerable<MemberInfo> GetSourceMembers(Type t)
	{
		return GetMembers(t).Where(m => m.MemberType is MemberTypes.Field or MemberTypes.Property or MemberTypes.Method);
	}

	/// <summary>
	///   Gets the type data container description.
	/// </summary>
	/// <param name="to">The to.</param>
	/// <exception cref="NotImplementedException"></exception>
	/// <returns><![CDATA[IEnumerable<KeyValuePair<string, Tuple<MemberInfo, Type>>>]]></returns>
	public static IEnumerable<KeyValuePair<string, Tuple<MemberInfo, Type>>> GetTypeDataContainerDescription(Type to)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	///   Gets the underlying type cache.
	/// </summary>
	/// <param name="t">The t.</param>
	/// <returns>A Type.</returns>
	public static Type GetUnderlyingTypeCache(this Type t)
	{
		return UnderlyingTypes.GetOrAdd(t, type => Nullable.GetUnderlyingType(type));
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="TAttribute"></typeparam>
	/// <param name="member">The member.</param>
	/// <returns>A bool.</returns>
	public static bool Has<TAttribute>(this MemberInfo member)
	  where TAttribute : Attribute
	{
		return member.IsDefined(Metadata<TAttribute>.Type);
	}

	/// <summary>
	///   Have the default constructor.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>A bool.</returns>
	public static bool HasDefaultConstructor(Type type)
	{
		return type.GetConstructor(Type.EmptyTypes) != null;
	}

	/// <summary>
	///   Is nullable.
	/// </summary>
	/// <param name="t">The t.</param>
	/// <returns>A bool.</returns>
	public static bool IsNullable(Type t)
	{
		return IsNullableCache.GetOrAdd(
		  t,
		  type => type.IsGenericType && type.GetGenericTypeDefinitionCache() == Metadata.Nullable1);
	}

	/// <summary>
	///   Is public.
	/// </summary>
	/// <param name="propertyInfo">The property info.</param>
	/// <returns>A bool.</returns>
	public static bool IsPublic(this PropertyInfo propertyInfo)
	{
		return (propertyInfo.GetGetMethod() ?? propertyInfo.GetSetMethod()) != null;
	}

	/// <summary>
	///   Sets the member value.
	/// </summary>
	/// <param name="propertyOrField">The property or field.</param>
	/// <param name="target">The target.</param>
	/// <param name="value">The value.</param>
	public static void SetMemberValue(this MemberInfo propertyOrField, object target, object value)
	{
		if (propertyOrField is PropertyInfo property)
		{
			property.SetValue(target, value, null);

			return;
		}

		if (propertyOrField is FieldInfo field)
		{
			field.SetValue(target, value);

			return;
		}

		throw Expected(propertyOrField);
	}

	/// <summary>
	///   Expecteds the <see cref="ArgumentOutOfRangeException" />.
	/// </summary>
	/// <param name="propertyOrField">The property or field.</param>
	/// <returns>An ArgumentOutOfRangeException.</returns>
	private static ArgumentOutOfRangeException Expected(MemberInfo propertyOrField)
	{
		return new ArgumentOutOfRangeException(
		  nameof(propertyOrField),
		  "Expected a property or field, not " + propertyOrField);
	}

	/// <summary>
	///   根据类型获取成员信息(字段与属性)
	/// </summary>
	/// <param name="t"></param>
	/// <returns></returns>
	private static IEnumerable<MemberInfo> GetDestinationMembers(Type t)
	{
		return GetMembers(t).Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property);
	}

	/// <summary>
	///   根据类型获取成员信息(字段、属性、方法)
	/// </summary>
	/// <param name="t"></param>
	/// <returns></returns>
	private static IEnumerable<MemberInfo> GetMembers(IReflect t)
	{
		return t.GetMembers(BindingFlagsInstancePublic);
	}

	/// <summary>
	///   The matched member.
	/// </summary>
	public class MatchedMember
	{
		/// <summary>
		///   Initializes a new instance of the <see cref="MatchedMember" /> class.
		/// </summary>
		/// <param name="first">The first.</param>
		/// <param name="second">The second.</param>
		public MatchedMember(MemberInfo first, MemberInfo second)
		{
			First = first;
			Second = second;
		}

		/// <summary>
		///   Gets or Sets the first.
		/// </summary>
		public MemberInfo First { get; set; }

		/// <summary>
		///   Gets or Sets the second.
		/// </summary>
		public MemberInfo Second { get; set; }
	}
}