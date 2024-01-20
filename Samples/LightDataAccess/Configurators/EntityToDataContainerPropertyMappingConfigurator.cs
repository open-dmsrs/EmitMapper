using System.Reflection;

namespace LightDataAccess.Configurators;

/// <summary>
///   The item configuration.
/// </summary>
public class EntityToDataContainerPropertyMappingConfigurator : DefaultMapConfig
{
	/// <summary>
	///   Initializes a new instance of the <see cref="EntityToDataContainerPropertyMappingConfigurator" /> class.
	/// </summary>
	public EntityToDataContainerPropertyMappingConfigurator()
	{
		ConstructBy(() => new DataContainer { Fields = new Dictionary<string, string>() });
	}

	/// <summary>
	///   Gets the mapping operations.
	/// </summary>
	/// <param name="from">The type from.</param>
	/// <param name="to">To type to.</param>
	/// <returns>The mapping operations.</returns>
	public override IEnumerable<IMappingOperation> GetMappingOperations(Type from, Type to)
	{
		return FilterOperations(
		  from,
		  to,
		  ReflectionHelper.GetPublicFieldsAndProperties(from)
			.Where(
			  member => (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
						&& ((PropertyInfo)member).GetGetMethod() is not null).Select(
			  sourceMember => (IMappingOperation)new SrcReadOperation
			  {
				  Source = new MemberDescriptor(sourceMember),
				  Setter = (destination, value, state) =>
			  {
					if (destination is null || value is null
										|| destination is not DataContainer
										  container)
				  {
					  return;
				  }

				  var sourceType =
				  ReflectionHelper.GetMemberReturnType(sourceMember);

					var fieldsDescription =
				  ReflectionHelper.GetDataMemberDefinition(sourceMember);

					ConvertSourcePropertyToFields(
				  value,
				  sourceType,
				  container,
				  (List<Tuple<string, Type>>)fieldsDescription);
				}
			  })).ToArray();
	}

	/// <summary>
	///   Converts the source property to fields.
	/// </summary>
	/// <param name="sourceType">Type of the property.</param>
	/// <param name="sourceValue">The property value.</param>
	/// <param name="container">The container.</param>
	/// <param name="fieldsDescription">The fields description.</param>
	private static void ConvertSourcePropertyToFields(
	  object sourceValue,
	  Type sourceType,
	  DataContainer container,
	  List<Tuple<string, Type>> fieldsDescription)
	{
		if (container is null || container.Fields is null)
		{
			return;
		}

		fieldsDescription.ForEach(
		  fd =>
		  {
			  if (container.Fields.ContainsKey(fd.Item1))
			  {
				  return;
			  }

			  var value = ReflectionHelper.ConvertValue(sourceValue, sourceType, fd.Item2);

			  if (value is not null)
			  {
				  container.Fields.Add(fd.Item1, value.ToString());
			  }
		  });
	}
}