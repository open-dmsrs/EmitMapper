using System.Reflection;

namespace EMConfigurations;

/// <summary>
///   The flattering config.
/// </summary>
public class FlatteringConfig : DefaultMapConfig
{
  protected Func<string, string, bool> NestedMembersMatcher;

  /// <summary>
  ///   Initializes a new instance of the <see cref="FlatteringConfig" /> class.
  /// </summary>
  public FlatteringConfig()
  {
    NestedMembersMatcher = (m1, m2) => m1.StartsWith(m2);
  }

  /// <summary>
  ///   Gets the mapping operations.
  /// </summary>
  /// <param name="from">The from.</param>
  /// <param name="to">The to.</param>
  /// <returns><![CDATA[IEnumerable<IMappingOperation>]]></returns>
  public override IEnumerable<IMappingOperation> GetMappingOperations(Type from, Type to)
  {
    var destinationMembers = GetFieldsPropertiesMembers(to);
    var sourceMembers = GetSourceMembers(from);

    var result = destinationMembers
      .Select(dest => new { dest, matchedChain = GetMatchedChain(dest.Name, sourceMembers) }).Select(
        x => new ReadWriteSimple
        {
          Source = new MemberDescriptor(x.matchedChain), Destination = new MemberDescriptor(new[] { x.dest })
        });

    return FilterOperations(from, to, result);
  }

  /// <summary>
  ///   Matches the nested members.
  /// </summary>
  /// <param name="nestedMembersMatcher">The nested members matcher.</param>
  /// <returns>A DefaultMapConfig.</returns>
  public DefaultMapConfig MatchNestedMembers(Func<string, string, bool> nestedMembersMatcher)
  {
    NestedMembersMatcher = nestedMembersMatcher;

    return this;
  }

  /// <summary>
  ///   Gets the all members.
  /// </summary>
  /// <param name="t">The t.</param>
  /// <returns><![CDATA[IEnumerable<MemberInfo>]]></returns>
  private static IEnumerable<MemberInfo> GetAllMembers(Type t)
  {
    var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

    return t.GetMembers(bindingFlags);
  }

  /// <summary>
  ///   Gets the fields properties members.
  /// </summary>
  /// <param name="t">The t.</param>
  /// <returns><![CDATA[IEnumerable<MemberInfo>]]></returns>
  private static IEnumerable<MemberInfo> GetFieldsPropertiesMembers(Type t)
  {
    return GetAllMembers(t).Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property);
  }

  /// <summary>
  ///   Gets the source members.
  /// </summary>
  /// <param name="t">The t.</param>
  /// <returns><![CDATA[IEnumerable<MemberInfo>]]></returns>
  private static IEnumerable<MemberInfo> GetSourceMembers(Type t)
  {
    return GetAllMembers(t).Where(
      m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property
                                             || m.MemberType == MemberTypes.Method);
  }

  /// <summary>
  ///   Gets the source sub members.
  /// </summary>
  /// <param name="mi">The mi.</param>
  /// <returns><![CDATA[IEnumerable<MemberInfo>]]></returns>
  private static IEnumerable<MemberInfo> GetSourceSubMembers(MemberInfo mi)
  {
    var t = ReflectionHelper.GetMemberReturnType(mi);

    // Type t;
    // if (mi.MemberType == MemberTypes.Field)
    // t = mi.DeclaringType.GetField(mi.Name).FieldType;
    // else
    // t = mi.DeclaringType.GetProperty(mi.Name).PropertyType;
    return GetFieldsPropertiesMembers(t);
  }

  /// <summary>
  ///   Gets the matched chain.
  /// </summary>
  /// <param name="destName">The dest name.</param>
  /// <param name="sourceMembers">The source members.</param>
  /// <exception cref="EmitMapperException"></exception>
  /// <returns><![CDATA[IEnumerable<MemberInfo>]]></returns>
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
      var matchedChain = GetMatchedChain(
        destName.Substring(sourceMemberInfo.Name.Length),
        GetSourceSubMembers(sourceMemberInfo));

      if (matchedChain != null && matchedChain.Any())
        result.AddRange(matchedChain);
      else

        // todo: need to add filter logic like DefaultMapConfig
        throw new EmitMapperException(
          $" The member '{destName}' of target members can not match any in source member '{sourceMemberInfo.Name}'."
          + $" pls ignore it or delete '{destName}' in the target object."
          + $" or add new member '{destName.Substring(sourceMemberInfo.Name.Length)}' in source class '{ReflectionHelper.GetMemberReturnType(sourceMemberInfo).FullName}'.");
    }

    return result;
  }
}