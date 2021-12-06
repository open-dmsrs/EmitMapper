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

    public MemberInfo MemberInfo
        /* Unmerged change from project 'EmitMapper (netstandard2.1)'
        Before:
                {
                    get
                    {
                        return MembersChain == null || MembersChain.Length == 0 ? null : MembersChain[MembersChain.Length - 1];
                    }
                }
        After:
                {
                    get
                    {
                        return MembersChain == null || MembersChain.Length == 0 ? null : MembersChain[MembersChain.Length - 1];
                    }
                }
        */ =>
        this.MembersChain == null || this.MembersChain.Length == 0
            ? null
            : this.MembersChain[this.MembersChain.Length - 1];

    public Type MemberType
        /* Unmerged change from project 'EmitMapper (netstandard2.1)'
        Before:
                {
                    get
                    {
                        return ReflectionUtils.GetMemberType(MemberInfo);
                    }
                }
        After:
                {
                    get
                    {
                        return ReflectionUtils.GetMemberType(MemberInfo);
                    }
                }
        */ =>
        ReflectionUtils.GetMemberType(this.MemberInfo);

    public override string ToString()
    {
        return "[" + this.MembersChain.Select(mc => ReflectionUtils.GetMemberType(mc).Name + ":" + mc.Name).ToCSV(",")
                   + "]";
    }
}