using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Utils;
using Microsoft.AspNetCore.Http;

namespace EmitMapper.Mvc.Net;

/// <summary>
///   The form collection map config.
/// </summary>
public class FormCollectionMapConfig : MapConfigBaseImpl
{
	/// <summary>
	///   Gets the configuration name.
	/// </summary>
	/// <returns>A string.</returns>
	public override string? GetConfigurationName()
	{
		return null;
	}

	/// <summary>
	///   Gets the mapping operations.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="to">The to.</param>
	/// <returns><![CDATA[IEnumerable<IMappingOperation>]]></returns>
	public override IEnumerable<IMappingOperation> GetMappingOperations(Type from, Type to)
	{
		var members = ReflectionHelper.GetPublicFieldsAndProperties(to);

		return members.Select(
		  m => (IMappingOperation)new DestWriteOperation
		  {
			  Destination = new MemberDescriptor(m),
			  Getter = (ValueGetter<object>)((form, valueProviderObj) =>
		  {
				if (((FormCollection)form).TryGetValue(
				  m.Name,
				  out var res))
			  {
				  return ValueToWrite<object>.ReturnValue(
				  Convert(
					new ValueProviderResult(res),
					ReflectionHelper.GetMemberReturnType(m)));
			  }

			  return ValueToWrite<object>.Skip();
			})
		  }).ToArray();
	}

	/// <summary>
	///   Gets the root mapping operation.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="to">The to.</param>
	/// <returns>An IRootMappingOperation.</returns>
	public override IRootMappingOperation? GetRootMappingOperation(Type from, Type to)
	{
		return null;
	}

	/// <summary>
	///   Gets the static converters manager.
	/// </summary>
	/// <returns>A StaticConvertersManager.</returns>
	public override StaticConvertersManager? GetStaticConvertersManager()
	{
		return null;
	}

	/// <summary>
	/// </summary>
	/// <param name="valueProviderResult">The value provider result.</param>
	/// <param name="type">The type.</param>
	/// <exception cref="NotImplementedException"></exception>
	/// <returns>An object.</returns>
	private object Convert(ValueProviderResult valueProviderResult, Type type)
	{
		throw new NotImplementedException();
	}
}