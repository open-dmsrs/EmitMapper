using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmitMapper.Utils;

namespace EmitMapper.MappingConfiguration;

public class MemberDescriptor
{
  public MemberDescriptor(MemberInfo singleMember)
  {
    MembersChain = Enumerable.Repeat(singleMember, 1);
  }

  public MemberDescriptor(IEnumerable<MemberInfo> membersChain)
  {
    MembersChain = membersChain;
  }

  public IEnumerable<MemberInfo> MembersChain { get; set; }

  public MemberInfo MemberInfo => MembersChain.LastOrDefault();

  public Type MemberType => ReflectionHelper.GetMemberReturnType(MemberInfo);

  public override string ToString()
  {
    return "[" + MembersChain.Select(mc => ReflectionHelper.GetMemberReturnType(mc).Name + ":" + mc.Name).ToCsv(",")
               + "]";
  }
}