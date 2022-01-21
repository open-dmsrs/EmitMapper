using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmitMapper.Utils;

namespace EmitMapper.MappingConfiguration;

public class MemberDescriptor
{
  private IEnumerable<MemberInfo> _membersChain;

  public MemberDescriptor(MemberInfo singleMember)
  {
    MembersChain = Enumerable.Repeat(singleMember, 1);
  }

  public MemberDescriptor(IEnumerable<MemberInfo> membersChain)
  {
    MembersChain = membersChain;
  }

  public IEnumerable<MemberInfo> MembersChain
  {
    get { return _membersChain; }
    set
    {
      _membersChain = value;
      MemberInfo = _membersChain.LastOrDefault();
      MemberType = ReflectionHelper.GetMemberReturnType(MemberInfo);
    }
  }

  public MemberInfo MemberInfo { get; private set; }

  public Type MemberType { get; private set; }

  public override string ToString()
  {
    return "[" + MembersChain.Select(mc => ReflectionHelper.GetMemberReturnType(mc).Name + ":" + mc.Name).ToCsv(",")
               + "]";
  }
}