﻿using System;
using System.Collections.Generic;
using System.Linq;
using EmitMapper.Conversion;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
using EmitMapper.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EmitMapper.Mvc.Net;

public class FormCollectionMapConfig : MapConfigBaseImpl
{
  public override string GetConfigurationName()
  {
    return null;
  }

  public override IEnumerable<IMappingOperation> GetMappingOperations(Type from, Type to)
  {
    var members = ReflectionHelper.GetPublicFieldsAndProperties(to);

    return members.Select(
      m => (IMappingOperation)new DestWriteOperation
      {
        Destination = new MemberDescriptor(m),
        Getter = (ValueGetter<object>)((form, valueProviderObj) =>
        {
          if (((FormCollection)form).TryGetValue(
                m.Name,
                out var res))
            return ValueToWrite<object>.ReturnValue(
              Convert(
                new ValueProviderResult(res),
                ReflectionHelper.GetMemberReturnType(m)));

          return ValueToWrite<object>.Skip();
        })
      }).ToArray();
  }

  public override IRootMappingOperation GetRootMappingOperation(Type from, Type to)
  {
    return null;
  }

  public override StaticConvertersManager GetStaticConvertersManager()
  {
    return null;
  }

  private object Convert(ValueProviderResult valueProviderResult, Type type)
  {
    throw new NotImplementedException();
  }
}