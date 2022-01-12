using System;
using System.Collections.Generic;
using System.Reflection;

namespace EmitMapper.Utils;

public class ProfileMap
{
    public bool AllowNullCollections { get; }
    public bool AllowNullDestinationValues { get; }
    public bool ConstructorMappingEnabled { get; }
    public bool EnableNullPropagationForQueryMapping { get; }
    public bool MethodMappingEnabled { get; }
    public bool FieldMappingEnabled { get; }
    public string Name { get; }
    public Func<FieldInfo, bool> ShouldMapField { get; }
    public Func<PropertyInfo, bool> ShouldMapProperty { get; }
    public Func<MethodInfo, bool> ShouldMapMethod { get; }
    public Func<ConstructorInfo, bool> ShouldUseConstructor { get; }

    public IReadOnlyCollection<string> GlobalIgnores { get; }
    public IEnumerable<MethodInfo> SourceExtensionMethods { get; }
    public List<string> Prefixes { get; }
    public List<string> Postfixes { get; }
}