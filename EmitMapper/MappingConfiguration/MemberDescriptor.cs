using EmitMapper.Utils;
using System;
using System.Linq;
using System.Reflection;

namespace EmitMapper.MappingConfiguration
{
    public class MemberDescriptor
    {
        public MemberDescriptor(MemberInfo singleMember)
        {
            MembersChain = new[] { singleMember };
        }

        public MemberDescriptor(MemberInfo[] membersChain)
        {
            MembersChain = membersChain;
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
 */
 => MembersChain == null || MembersChain.Length == 0 ? null : MembersChain[MembersChain.Length - 1];

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
 */
 => ReflectionUtils.GetMemberType(MemberInfo);

        public override string ToString()
        {
            return "[" + MembersChain.Select(mc => ReflectionUtils.GetMemberType(mc).Name + ":" + mc.Name).ToCSV(",") + "]";
        }
    }
}
