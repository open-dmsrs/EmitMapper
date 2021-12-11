namespace EmitMapper.Mvc.Net
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using EmitMapper.Conversion;
    using EmitMapper.MappingConfiguration;
    using EmitMapper.MappingConfiguration.MappingOperations;
    using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
    using EmitMapper.Utils;

    public class FormCollectionMapConfig : IMappingConfigurator
    {
        public IMappingOperation[] GetMappingOperations(Type from, Type to)
        {
            var members = ReflectionUtils.GetPublicFieldsAndProperties(to);
            return members.Select(
                m => new DestWriteOperation
                         {
                             Destination = new MemberDescriptor(m),
                             Getter = (ValueGetter<object>)((form, valueProviderObj) =>
                                                                   {
                                                                       var valueProvider =
                                                                           valueProviderObj as IValueProvider;
                                                                       if (valueProvider == null)
                                                                           valueProvider = ((FormCollection)form)
                                                                               .ToValueProvider();

                                                                       var res = valueProvider.GetValue(m.Name);
                                                                       // here need to check the value of res is not null, then can convert the value
                                                                       if (res != null)
                                                                           return ValueToWrite<object>.ReturnValue(
                                                                               res.ConvertTo(
                                                                                   ReflectionUtils.GetMemberType(m)));
                                                                       return ValueToWrite<object>.Skip();
                                                                   })
                         }).ToArray();
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