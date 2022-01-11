using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmitMapper;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
using EmitMapper.Utils;

namespace EMConfigurations;

public class FlatteringConfig : DefaultMapConfig
{
  protected Func<string, string, bool> NestedMembersMatcher;

  public FlatteringConfig()
  {
    NestedMembersMatcher = (m1, m2) => m1.StartsWith(m2);
  }

  public override IMappingOperation[] GetMappingOperations(Type from, Type to)
  {
    var destinationMembers = GetFieldsPropertiesMembers(to);
    var sourceMembers = GetSourceMembers(from);
    var result = new List<IMappingOperation>();
    foreach (var dest in destinationMembers)
    {
      var matchedChain = GetMatchedChain(dest.Name, sourceMembers).ToArray();
      if (matchedChain == null || matchedChain.Length == 0) continue;
      result.Add(
        new ReadWriteSimple
        {
          Source = new MemberDescriptor(matchedChain),
          Destination = new MemberDescriptor(new[] { dest })
        }
      );
    }

    return result.ToArray();
  }

  public DefaultMapConfig MatchNestedMembers(Func<string, string, bool> nestedMembersMatcher)
  {
    NestedMembersMatcher = nestedMembersMatcher;
    return this;
  }

  private IEnumerable<MemberInfo> GetMatchedChain(string destName, IEnumerable<MemberInfo> sourceMembers)
  {
    var sourceMatches =
      sourceMembers.Where(s => MatchMembers(destName, s.Name) || NestedMembersMatcher(destName, s.Name));
    var len = 0;
    MemberInfo sourceMemberInfo = null;
    foreach (var mi in sourceMatches)
      if (mi.Name.Length > len)
      {
        len = mi.Name.Length;
        sourceMemberInfo = mi;
      }

    if (sourceMemberInfo == null) return null;
    var result = new List<MemberInfo> { sourceMemberInfo };
    if (!MatchMembers(destName, sourceMemberInfo.Name))
    {
      var matchedChain = GetMatchedChain(destName.Substring(sourceMemberInfo.Name.Length), GetSourceSubMembers(sourceMemberInfo));
      if (matchedChain != null && matchedChain.Any())
        result.AddRange(matchedChain);
      else
      {
        throw new EmitMapperException(
          $" The member '{destName}' of target members can not match any in source member '{sourceMemberInfo.Name}'." +
          $" pls ignore it or delete '{destName}' in the target object." +
          $" or add new member '{destName.Substring(sourceMemberInfo.Name.Length)}' in source class '{ReflectionUtils.GetMemberReturnType(sourceMemberInfo).FullName}'.");
      }
    }
    return result;
  }

  private static IEnumerable<MemberInfo> GetSourceMembers(Type t)
  {
    return GetAllMembers(t)
      .Where(
        m =>
          m.MemberType == MemberTypes.Field ||
          m.MemberType == MemberTypes.Property ||
          m.MemberType == MemberTypes.Method
      );
  }

  private static IEnumerable<MemberInfo> GetSourceSubMembers(MemberInfo mi)
  {
    //Type t = ReflectionUtils.GetMemberReturnType(mi);

    Type t;
    if (mi.MemberType == MemberTypes.Field)
      t = mi.DeclaringType.GetField(mi.Name).FieldType;
    else
      t = mi.DeclaringType.GetProperty(mi.Name).PropertyType;
    return GetFieldsPropertiesMembers(t);
  }

  private static IEnumerable<MemberInfo> GetFieldsPropertiesMembers(Type t)
  {
    return GetAllMembers(t).Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property);
  }

  private static IEnumerable<MemberInfo> GetAllMembers(Type t)
  {
    var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
    return t.GetMembers(bindingFlags);
  }
}