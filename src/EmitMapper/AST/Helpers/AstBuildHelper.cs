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
	public static IAstRefOrValue ICallMethod(
	  MethodInfo methodInfo,
	  IAstRefOrAddr invocationObject,
	  List<IAstStackItem>? arguments)
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
	public static IAstRefOrValue ICastClass(IAstRefOrValue value, Type targetType)
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
	public static IAstRefOrAddr IReadArgumentRa(int argumentIndex, Type argumentType)
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
	public static IAstRefOrValue IReadArgumentRv(int argumentIndex, Type? argumentType)
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
	public static IAstRefOrAddr IReadArrayItemRa(IAstRef array, int index)
	{
		switch (array?.ItemType.IsValueType)
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
	public static IAstRefOrValue IReadArrayItemRv(IAstRef array, int index)
	{
		switch (array?.ItemType.IsValueType)
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
	public static IAstRefOrAddr IReadFieldRa(IAstRefOrAddr sourceObject, FieldInfo fieldInfo)
	{
		switch (fieldInfo?.FieldType.IsValueType)
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
	public static IAstRefOrValue IReadFieldRv(IAstRefOrAddr sourceObject, FieldInfo fieldInfo)
	{
		switch (fieldInfo?.FieldType.IsValueType)
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
	public static IAstRefOrAddr IReadLocalRa(LocalBuilder loc)
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
	public static IAstRefOrValue IReadLocalRv(LocalBuilder loc)
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
	public static IAstStackItem IReadMember(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
	{
		switch (memberInfo.MemberType)
		{
			case MemberTypes.Method:
			{
				var methodInfo = memberInfo.DeclaringType.GetMethodCache(memberInfo.Name);

				switch (methodInfo?.ReturnType)
				{
					case null:
						throw new EmitMapperException("Invalid member:" + memberInfo?.Name);
				}

				switch (methodInfo.GetParameters().Length)
				{
					case > 0:
						throw new EmitMapperException("Method " + memberInfo?.Name + " should not have parameters");
					default:
						return (IAstRef)ICallMethod(methodInfo, sourceObject, null);
				}
			}
			case MemberTypes.Field:
				return IReadFieldRa(sourceObject, (FieldInfo)memberInfo);
			default:
				return (IAstRef)IReadPropertyRv(sourceObject, (PropertyInfo)memberInfo);
		}
	}

	/// <summary>
	///   Reads the member r a.
	/// </summary>
	/// <param name="sourceObject">The source object.</param>
	/// <param name="memberInfo">The member info.</param>
	/// <exception cref="EmitMapperException"></exception>
	/// <returns>An IAstRefOrAddr.</returns>
	public static IAstRefOrAddr IReadMemberRa(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
	{
		switch (memberInfo.MemberType)
		{
			case MemberTypes.Method:
			{
				var methodInfo = memberInfo.DeclaringType?.GetMethodCache(memberInfo.Name);

				if (methodInfo?.ReturnType == null)
				{
					throw new EmitMapperException("Invalid member:" + memberInfo?.Name);
				}

				if (methodInfo.GetParameters().Length > 0)
				{
					throw new EmitMapperException("Method " + memberInfo.Name + " should not have parameters");
				}

				if (methodInfo?.ReturnType?.IsValueType != false)
				{
					var name = memberInfo?.Name;

					throw new EmitMapperException("Method " + name + " should return a reference");
				}

				return (IAstRef)ICallMethod(methodInfo, sourceObject, null);
			}
			case MemberTypes.Field:
				return IReadFieldRa(sourceObject, (FieldInfo)memberInfo);
		}

		var pi = (PropertyInfo)memberInfo;

		if (pi?.PropertyType.IsValueType == true)
		{
			return IReadPropertyRa(sourceObject, (PropertyInfo)memberInfo);
		}

		return (IAstRef)IReadPropertyRv(sourceObject, (PropertyInfo)memberInfo);
	}

	/// <summary>
	///   Reads the member r v.
	/// </summary>
	/// <param name="sourceObject">The source object.</param>
	/// <param name="memberInfo">The member info.</param>
	/// <exception cref="EmitMapperException"></exception>
	/// <returns>An IAstRefOrValue.</returns>
	public static IAstRefOrValue IReadMemberRv(IAstRefOrAddr sourceObject, MemberInfo? memberInfo)
	{
		switch (memberInfo?.MemberType)
		{
			case MemberTypes.Method:
			{
				var methodInfo = memberInfo?.DeclaringType?.GetMethodCache(memberInfo.Name);

				switch (methodInfo?.ReturnType)
				{
					case null:
						throw new EmitMapperException("Invalid member:" + memberInfo?.Name);
				}

				switch (methodInfo.GetParameters().Length)
				{
					case > 0:
						throw new EmitMapperException("Method " + memberInfo?.Name + " should not have parameters");
					default:
						return ICallMethod(methodInfo, sourceObject, null);
				}
			}
			case MemberTypes.Field:
				return IReadFieldRv(sourceObject, (FieldInfo)memberInfo);
			default:
				return IReadPropertyRv(sourceObject, memberInfo as PropertyInfo);
		}
	}

	/// <summary>
	///   Reads the members chain.
	/// </summary>
	/// <param name="sourceObject">The source object.</param>
	/// <param name="membersChain">The members chain.</param>
	/// <returns>An IAstRefOrValue.</returns>
	public static IAstRefOrValue IReadMembersChain(IAstRefOrAddr sourceObject, IEnumerable<MemberInfo> membersChain)
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
			src = IReadMemberRa(src, cur);
			cur = enumerator.Current;
		}

		return IReadMemberRv(src, cur);
	}

	/// <summary>
	///   Reads the members chain.
	/// </summary>
	/// <param name="sourceObject">The source object.</param>
	/// <param name="membersChainOfOne">The members chain of one.</param>
	/// <returns>An IAstRefOrValue.</returns>
	public static IAstRefOrValue IReadMembersChain(IAstRefOrAddr sourceObject, MemberInfo? membersChainOfOne)
	{
		return IReadMemberRv(sourceObject, membersChainOfOne);
	}

	/// <summary>
	///   Reads the property r a.
	/// </summary>
	/// <param name="sourceObject">The source object.</param>
	/// <param name="propertyInfo">The property info.</param>
	/// <returns>An IAstRefOrAddr.</returns>
	public static IAstRefOrAddr IReadPropertyRa(IAstRefOrAddr sourceObject, PropertyInfo propertyInfo)
	{
		switch (propertyInfo?.PropertyType.IsValueType)
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
	public static IAstRefOrValue IReadPropertyRv(IAstRefOrAddr sourceObject, PropertyInfo? propertyInfo)
	{
		switch (propertyInfo?.PropertyType.IsValueType)
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
	/// <returns>IAstRefOrAddr</returns>
	public static IAstRefOrAddr IReadThis(Type thisType)
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
	public static IAstNode IWriteMember(MemberInfo memberInfo, IAstRefOrAddr targetObject, IAstRefOrValue value)
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
	public static IAstNode IWriteMembersChain(
	  IEnumerable<MemberInfo>? membersChain,
	  IAstRefOrAddr targetObject,
	  IAstRefOrValue value)
	{
		var readTarget = targetObject;
		var enumerator = membersChain?.GetEnumerator();
		MemberInfo? cur = null;

		if (enumerator is not null && enumerator.MoveNext())
		{
			cur = enumerator?.Current;
		}

		while (enumerator is not null && enumerator.MoveNext())
		{
			readTarget = IReadMemberRa(readTarget, cur);
			cur = enumerator?.Current;
		}

		return IWriteMember(cur, readTarget, value);
	}
}