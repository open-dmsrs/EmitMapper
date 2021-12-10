namespace EmitMapper.MappingConfiguration;

using System;
using System.Linq;
using System.Reflection;

using EmitMapper.Utils;

public class MemberDescriptor
{
    public MemberDescriptor(MemberInfo singleMember)
    {
        this.MembersChain = new[] { singleMember };
    }

    public MemberDescriptor(MemberInfo[] membersChain)
    {
        this.MembersChain = membersChain;
    }

    public MemberInfo[] MembersChain { get; set; }

    public MemberInfo MemberInfo =>
        this.MembersChain == null || this.MembersChain.Length == 0
            ? null
            : this.MembersChain[this.MembersChain.Length - 1];

    public Type MemberType => ReflectionUtils.GetMemberType(this.MemberInfo);

    public override string ToString()
    {
        return "[" + this.MembersChain.Select(mc => ReflectionUtils.GetMemberType(mc).Name + ":" + mc.Name).ToCSV(",")
                   + "]";
    }
}