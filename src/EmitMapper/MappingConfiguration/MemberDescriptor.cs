namespace EmitMapper.MappingConfiguration;

/// <summary>
///   The member descriptor.
/// </summary>
public class MemberDescriptor
{
	private IEnumerable<MemberInfo> membersChain;

	/// <summary>
	///   Initializes a new instance of the <see cref="MemberDescriptor" /> class.
	/// </summary>
	/// <param name="singleMember">The single member.</param>
	public MemberDescriptor(MemberInfo singleMember)
	{
		MembersChain = Enumerable.Repeat(singleMember, 1);
	}

	/// <summary>
	///   Initializes a new instance of the <see cref="MemberDescriptor" /> class.
	/// </summary>
	/// <param name="membersChain">The members chain.</param>
	public MemberDescriptor(IEnumerable<MemberInfo> membersChain)
	{
		MembersChain = membersChain;
	}

	/// <summary>
	///   Gets the member info.
	/// </summary>
	public MemberInfo MemberInfo { get; private set; }

	/// <summary>
	///   Gets or Sets the members chain.
	/// </summary>
	public IEnumerable<MemberInfo> MembersChain
	{
		get => membersChain;
		set
		{
			membersChain = value;
			MemberInfo = membersChain.LastOrDefault();
			MemberType = ReflectionHelper.GetMemberReturnType(MemberInfo);
		}
	}

	/// <summary>
	///   Gets the member type.
	/// </summary>
	public Type MemberType { get; private set; }

	/// <summary>
	///   Tos the string.
	/// </summary>
	/// <returns>A string.</returns>
	public override string ToString()
	{
		return "[" + MembersChain.Select(mc => ReflectionHelper.GetMemberReturnType(mc).Name + ":" + mc.Name).ToCsv(",")
				   + "]";
	}
}