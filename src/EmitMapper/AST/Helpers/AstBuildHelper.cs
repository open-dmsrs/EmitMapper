namespace EmitMapper.AST.Helpers;

/// <summary>
///   The ast build helper.
/// </summary>
internal static class AstBuildHelper
{
	/// <summary>
	///   Calls the method.
	/// </summary>
	/// <param name="methodInfo">The method info.</param>
	/// <param name="invocationObject">The invocation object.</param>
	/// <param name="arguments">The arguments.</param>
	/// <returns>An IAstRefOrValue.</returns>
	public static IAstRefOrValue CallMethod(
	  MethodInfo methodInfo,
	  IAstRefOrAddr invocationObject,
	  List<IAstStackItem> arguments)
	{
		switch (methodInfo.ReturnType.IsValueType)
		{
			case true:
				return new AstCallMethodValue(methodInfo, invocationObject, arguments);
			default:
				return new AstCallMethodRef(methodInfo, invocationObject, arguments);
		}
	}

	/// <summary>
	///   Casts the class.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="targetType">The target type.</param>
	/// <returns>An IAstRefOrValue.</returns>
	public static IAstRefOrValue CastClass(IAstRefOrValue value, Type targetType)
	{
		switch (targetType.IsValueType)
		{
			case true:
				return new AstCastclassValue(value, targetType);
			default:
				return new AstCastclassRef(value, targetType);
		}
	}

	/// <summary>
	///   Reads the argument r a.
	/// </summary>
	/// <param name="argumentIndex">The argument index.</param>
	/// <param name="argumentType">The argument type.</param>
	/// <returns>An IAstRefOrAddr.</returns>
	public static IAstRefOrAddr ReadArgumentRa(int argumentIndex, Type argumentType)
	{
		switch (argumentType.IsValueType)
		{
			case true:
				return new AstReadArgumentAddr { ArgumentIndex = argumentIndex, ArgumentType = argumentType };
			default:
				return new AstReadArgumentRef { ArgumentIndex = argumentIndex, ArgumentType = argumentType };
		}
	}

	/// <summary>
	///   Reads the argument r v.
	/// </summary>
	/// <param name="argumentIndex">The argument index.</param>
	/// <param name="argumentType">The argument type.</param>
	/// <returns>An IAstRefOrValue.</returns>
	public static IAstRefOrValue ReadArgumentRv(int argumentIndex, Type? argumentType)
	{
		switch (argumentType.IsValueType)
		{
			case true:
				return new AstReadArgumentValue { ArgumentIndex = argumentIndex, ArgumentType = argumentType };
			default:
				return new AstReadArgumentRef { ArgumentIndex = argumentIndex, ArgumentType = argumentType };
		}
	}

	/// <summary>
	///   Reads the array item r a.
	/// </summary>
	/// <param name="array">The array.</param>
	/// <param name="index">The index.</param>
	/// <returns>An IAstRefOrAddr.</returns>
	public static IAstRefOrAddr ReadArrayItemRa(IAstRef array, int index)
	{
		switch (array.ItemType.IsValueType)
		{
			case true:
				return new AstReadArrayItemAddr { Array = array, Index = index };
			default:
				return new AstReadArrayItemRef { Array = array, Index = index };
		}
	}

	/// <summary>
	///   Reads the array item r v.
	/// </summary>
	/// <param name="array">The array.</param>
	/// <param name="index">The index.</param>
	/// <returns>An IAstRefOrValue.</returns>
	public static IAstRefOrValue ReadArrayItemRv(IAstRef array, int index)
	{
		switch (array.ItemType.IsValueType)
		{
			case true:
				return new AstReadArrayItemValue { Array = array, Index = index };
			default:
				return new AstReadArrayItemRef { Array = array, Index = index };
		}
	}

	/// <summary>
	///   Reads the field r a.
	/// </summary>
	/// <param name="sourceObject">The source object.</param>
	/// <param name="fieldInfo">The field info.</param>
	/// <returns>An IAstRefOrAddr.</returns>
	public static IAstRefOrAddr ReadFieldRa(IAstRefOrAddr sourceObject, FieldInfo fieldInfo)
	{
		switch (fieldInfo.FieldType.IsValueType)
		{
			case true:
				return new AstReadFieldAddr { FieldInfo = fieldInfo, SourceObject = sourceObject };
			default:
				return new AstReadFieldRef { FieldInfo = fieldInfo, SourceObject = sourceObject };
		}
	}

	/// <summary>
	///   Reads the field r v.
	/// </summary>
	/// <param name="sourceObject">The source object.</param>
	/// <param name="fieldInfo">The field info.</param>
	/// <returns>An IAstRefOrValue.</returns>
	public static IAstRefOrValue ReadFieldRv(IAstRefOrAddr sourceObject, FieldInfo fieldInfo)
	{
		switch (fieldInfo.FieldType.IsValueType)
		{
			case true:
				return new AstReadFieldValue { FieldInfo = fieldInfo, SourceObject = sourceObject };
			default:
				return new AstReadFieldRef { FieldInfo = fieldInfo, SourceObject = sourceObject };
		}
	}

	/// <summary>
	///   Reads the local r a.
	/// </summary>
	/// <param name="loc">The loc.</param>
	/// <returns>An IAstRefOrAddr.</returns>
	public static IAstRefOrAddr ReadLocalRa(LocalBuilder loc)
	{
		switch (loc.LocalType.IsValueType)
		{
			case true:
				return new AstReadLocalAddr(loc);
			default:
				return new AstReadLocalRef { LocalType = loc.LocalType, LocalIndex = loc.LocalIndex };
		}
	}

	/// <summary>
	///   Reads the local r v.
	/// </summary>
	/// <param name="loc">The loc.</param>
	/// <returns>An IAstRefOrValue.</returns>
	public static IAstRefOrValue ReadLocalRv(LocalBuilder loc)
	{
		switch (loc.LocalType.IsValueType)
		{
			case true:
				return new AstReadLocalValue { LocalType = loc.LocalType, LocalIndex = loc.LocalIndex };
			default:
				return new AstReadLocalRef { LocalType = loc.LocalType, LocalIndex = loc.LocalIndex };
		}
	}

	/// <summary>
	///   Reads the member.
	/// </summary>
	/// <param name="sourceObject">The source object.</param>
	/// <param name="memberInfo">The member info.</param>
	/// <exception cref="EmitMapperException"></exception>
	/// <returns>An IAstStackItem.</returns>
	public static IAstStackItem ReadMember(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
	{
		switch (memberInfo.MemberType)
		{
			case MemberTypes.Method:
			{
				var methodInfo = memberInfo.DeclaringType.GetMethodCache(memberInfo.Name);

				switch (methodInfo.ReturnType)
				{
					case null:
						throw new EmitMapperException("Invalid member:" + memberInfo.Name);
				}

				switch (methodInfo.GetParameters().Length)
				{
					case > 0:
						throw new EmitMapperException("Method " + memberInfo.Name + " should not have parameters");
					default:
						return (IAstRef)CallMethod(methodInfo, sourceObject, null);
				}
			}
			case MemberTypes.Field:
				return ReadFieldRa(sourceObject, (FieldInfo)memberInfo);
			default:
				return (IAstRef)ReadPropertyRv(sourceObject, (PropertyInfo)memberInfo);
		}
	}

	/// <summary>
	///   Reads the member r a.
	/// </summary>
	/// <param name="sourceObject">The source object.</param>
	/// <param name="memberInfo">The member info.</param>
	/// <exception cref="EmitMapperException"></exception>
	/// <returns>An IAstRefOrAddr.</returns>
	public static IAstRefOrAddr ReadMemberRa(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
	{
		switch (memberInfo.MemberType)
		{
			case MemberTypes.Method:
			{
				var methodInfo = memberInfo.DeclaringType.GetMethodCache(memberInfo.Name);

				switch (methodInfo.ReturnType)
				{
					case null:
						throw new EmitMapperException("Invalid member:" + memberInfo.Name);
				}

				switch (methodInfo.GetParameters().Length)
				{
					case > 0:
						throw new EmitMapperException("Method " + memberInfo.Name + " should not have parameters");
				}

				if (methodInfo.ReturnType?.IsValueType != false)
				{
					throw new EmitMapperException("Method " + memberInfo.Name + " should return a reference");
				}

				return (IAstRef)CallMethod(methodInfo, sourceObject, null);
			}
			case MemberTypes.Field:
				return ReadFieldRa(sourceObject, (FieldInfo)memberInfo);
		}

		var pi = (PropertyInfo)memberInfo;

		switch (pi.PropertyType.IsValueType)
		{
			case true:
				return ReadPropertyRa(sourceObject, (PropertyInfo)memberInfo);
			default:
				return (IAstRef)ReadPropertyRv(sourceObject, (PropertyInfo)memberInfo);
		}
	}

	/// <summary>
	///   Reads the member r v.
	/// </summary>
	/// <param name="sourceObject">The source object.</param>
	/// <param name="memberInfo">The member info.</param>
	/// <exception cref="EmitMapperException"></exception>
	/// <returns>An IAstRefOrValue.</returns>
	public static IAstRefOrValue ReadMemberRv(IAstRefOrAddr sourceObject, MemberInfo? memberInfo)
	{
		switch (memberInfo.MemberType)
		{
			case MemberTypes.Method:
			{
				var methodInfo = memberInfo.DeclaringType.GetMethodCache(memberInfo.Name);

				switch (methodInfo.ReturnType)
				{
					case null:
						throw new EmitMapperException("Invalid member:" + memberInfo.Name);
				}

				switch (methodInfo.GetParameters().Length)
				{
					case > 0:
						throw new EmitMapperException("Method " + memberInfo.Name + " should not have parameters");
					default:
						return CallMethod(methodInfo, sourceObject, null);
				}
			}
			case MemberTypes.Field:
				return ReadFieldRv(sourceObject, (FieldInfo)memberInfo);
			default:
				return ReadPropertyRv(sourceObject, (PropertyInfo)memberInfo);
		}
	}

	/// <summary>
	///   Reads the members chain.
	/// </summary>
	/// <param name="sourceObject">The source object.</param>
	/// <param name="membersChain">The members chain.</param>
	/// <returns>An IAstRefOrValue.</returns>
	public static IAstRefOrValue ReadMembersChain(IAstRefOrAddr sourceObject, IEnumerable<MemberInfo> membersChain)
	{
		var src = sourceObject;
		using var enumerator = membersChain.GetEnumerator();
		MemberInfo? cur = null;

		if (enumerator.MoveNext())
		{
			cur = enumerator.Current;
		}

		while (enumerator.MoveNext())
		{
			src = ReadMemberRa(src, cur);
			cur = enumerator.Current;
		}

		return ReadMemberRv(src, cur);
	}

	/// <summary>
	///   Reads the members chain.
	/// </summary>
	/// <param name="sourceObject">The source object.</param>
	/// <param name="membersChainOfOne">The members chain of one.</param>
	/// <returns>An IAstRefOrValue.</returns>
	public static IAstRefOrValue ReadMembersChain(IAstRefOrAddr sourceObject, MemberInfo? membersChainOfOne)
	{
		return ReadMemberRv(sourceObject, membersChainOfOne);
	}

	/// <summary>
	///   Reads the property r a.
	/// </summary>
	/// <param name="sourceObject">The source object.</param>
	/// <param name="propertyInfo">The property info.</param>
	/// <returns>An IAstRefOrAddr.</returns>
	public static IAstRefOrAddr ReadPropertyRa(IAstRefOrAddr sourceObject, PropertyInfo propertyInfo)
	{
		switch (propertyInfo.PropertyType.IsValueType)
		{
			case true:
				return new AstValueToAddr(new AstReadPropertyValue { SourceObject = sourceObject, PropertyInfo = propertyInfo });
			default:
				return new AstReadPropertyRef { SourceObject = sourceObject, PropertyInfo = propertyInfo };
		}
	}

	/// <summary>
	///   Reads the property r v.
	/// </summary>
	/// <param name="sourceObject">The source object.</param>
	/// <param name="propertyInfo">The property info.</param>
	/// <returns>An IAstRefOrValue.</returns>
	public static IAstRefOrValue ReadPropertyRv(IAstRefOrAddr sourceObject, PropertyInfo propertyInfo)
	{
		switch (propertyInfo.PropertyType.IsValueType)
		{
			case true:
				return new AstReadPropertyValue { SourceObject = sourceObject, PropertyInfo = propertyInfo };
			default:
				return new AstReadPropertyRef { SourceObject = sourceObject, PropertyInfo = propertyInfo };
		}
	}

	/// <summary>
	///   Reads the this.
	/// </summary>
	/// <param name="thisType">The this type.</param>
	/// <returns>An IAstRefOrAddr.</returns>
	public static IAstRefOrAddr ReadThis(Type? thisType)
	{
		switch (thisType.IsValueType)
		{
			case true:
				return new AstReadThisAddr { ThisType = thisType };
			default:
				return new AstReadThisRef { ThisType = thisType };
		}
	}

	/// <summary>
	///   Writes the member.
	/// </summary>
	/// <param name="memberInfo">The member info.</param>
	/// <param name="targetObject">The target object.</param>
	/// <param name="value">The value.</param>
	/// <returns>An IAstNode.</returns>
	public static IAstNode WriteMember(MemberInfo memberInfo, IAstRefOrAddr targetObject, IAstRefOrValue value)
	{
		switch (memberInfo.MemberType)
		{
			case MemberTypes.Field:
				return new AstWriteField { FieldInfo = (FieldInfo)memberInfo, TargetObject = targetObject, Value = value };
			default:
				return new AstWriteProperty(targetObject, value, (PropertyInfo)memberInfo);
		}
	}

	/// <summary>
	///   Writes the members chain.
	/// </summary>
	/// <param name="membersChain">The members chain.</param>
	/// <param name="targetObject">The target object.</param>
	/// <param name="value">The value.</param>
	/// <returns>An IAstNode.</returns>
	public static IAstNode WriteMembersChain(
	  IEnumerable<MemberInfo>? membersChain,
	  IAstRefOrAddr targetObject,
	  IAstRefOrValue value)
	{
		var readTarget = targetObject;
		var enumerator = membersChain?.GetEnumerator();
		MemberInfo? cur = null;

		if (enumerator is not null && enumerator.MoveNext())
		{
			cur = enumerator.Current;
		}

		while (enumerator is not null && enumerator.MoveNext())
		{
			readTarget = ReadMemberRa(readTarget, cur);
			cur = enumerator.Current;
		}

		return WriteMember(cur, readTarget, value);
	}
}