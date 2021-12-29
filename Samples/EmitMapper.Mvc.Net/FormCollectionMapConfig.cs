using System;
using System.Linq;
using EmitMapper.Conversion;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
using EmitMapper.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EmitMapper.Mvc.Net;

public class FormCollectionMapConfig : IMappingConfigurator
{
    public IMappingOperation[] GetMappingOperations(Type from, Type to)
    {
        var members = ReflectionUtils.GetPublicFieldsAndProperties(to);
        return members
            .Select(
                m =>
                    (IMappingOperation)new DestWriteOperation
                    {
                        Destination = new MemberDescriptor(m),
                        Getter =
                            (ValueGetter<object>)
                            (
                                (form, valueProviderObj) =>
                                {
                                    if (((FormCollection)form).TryGetValue(m.Name, out var res))
                                        return ValueToWrite<object>.ReturnValue(
                                            Convert(new ValueProviderResult(res), ReflectionUtils.GetMemberType(m)));
                                    return ValueToWrite<object>.Skip();
                                }
                            )
                    }
            )
            .ToArray();
    }

    public string GetConfigurationName()
    {
        return null;
    }

    public IRootMappingOperation GetRootMappingOperation(Type from, Type to)
    {
        return null;
    }

    public StaticConvertersManager GetStaticConvertersManager()
    {
        return null;
    }

    private object Convert(ValueProviderResult valueProviderResult, Type type)
    {
        throw new NotImplementedException();
    }
}