using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Utils;
using System;
using System.Linq;
using System.Web.Mvc;

namespace EmitMapper.Mvc.Net
{
    public class FormCollectionMapConfig : IMappingConfigurator
    {
        public IMappingOperation[] GetMappingOperations(Type from, Type to)
        {
            System.Reflection.MemberInfo[] members = ReflectionUtils.GetPublicFieldsAndProperties(to);
            return members
                .Select(
                    m =>
                    new DestWriteOperation()
                    {
                        Destination = new MemberDescriptor(m),
                        Getter =
                            (ValueGetter<object>)
                            (
                                (form, valueProviderObj) =>
                                {
                                    IValueProvider valueProvider = valueProviderObj as IValueProvider;
                                    if (valueProvider == null)
                                    {
                                        valueProvider = ((FormCollection)form).ToValueProvider();
                                    }

                                    ValueProviderResult res = valueProvider.GetValue(m.Name);
                                    // here need to check the value of res is not null, then can convert the value
                                    if (res != null)
                                    {
                                        return ValueToWrite<object>.ReturnValue(res.ConvertTo(ReflectionUtils.GetMemberType(m)));
                                    }
                                    else
                                    {
                                        return ValueToWrite<object>.Skip();
                                    }
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
    }
}
