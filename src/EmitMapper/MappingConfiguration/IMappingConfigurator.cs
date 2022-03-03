using System;
using System.Collections.Generic;
using EmitMapper.Conversion;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper.MappingConfiguration;
/// <summary>
/// The mapping configurator interface.
/// </summary>

public interface IMappingConfigurator
{
  /// <summary>
  ///   Define a custom constructor for the specified type
  /// </summary>
  /// <typeparam name="T">Type for which constructor is defining</typeparam>
  /// <param name="constructor">Custom constructor</param>
  /// <returns></returns>
  IMappingConfigurator ConstructBy<T>(TargetConstructor<T> constructor);

  /// <summary>
  ///   Define conversion for a generic. It is able to convert not one particular class but all generic family
  ///   providing a generic converter.
  /// </summary>
  /// <param name="from">Type of source. Can be also generic class or abstract array.</param>
  /// <param name="to">Type of destination. Can be also generic class or abstract array.</param>
  /// <param name="converterProvider">
  ///   Provider for getting detailed information about generic conversion.
  /// </param>
  /// <returns></returns>
  IMappingConfigurator ConvertGeneric(Type from, Type to, ICustomConverterProvider converterProvider);

  /// <summary>
  /// Converts the using.
  /// </summary>
  /// <typeparam name="TFrom"></typeparam>
  /// <typeparam name="TTo"></typeparam>
  /// <param name="converter">The converter.</param>
  /// <returns>An IMappingConfigurator.</returns>
  IMappingConfigurator ConvertUsing<TFrom, TTo>(Func<TFrom, TTo> converter);

  /// <summary>
  /// Filters the destination.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="valuesFilter">The values filter.</param>
  /// <returns>An IMappingConfigurator.</returns>
  IMappingConfigurator FilterDestination<T>(ValuesFilter<T> valuesFilter);

  /// <summary>
  /// Filters the source.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="valuesFilter">The values filter.</param>
  /// <returns>An IMappingConfigurator.</returns>
  IMappingConfigurator FilterSource<T>(ValuesFilter<T> valuesFilter);

  /// <summary>
  ///   Get unique configuration name to force Emit Mapper create new mapper instead using appropriate cached one.
  /// </summary>
  /// <returns></returns>
  string GetConfigurationName();

  /// <summary>
  ///   Get list of mapping operations. Each mapping mapping defines one coping operation from source to destination. For
  ///   this operation can be additionally defined the following custom operations:
  ///   - Custom getter which extracts values from source
  ///   - Custom values converter which converts extracted from source value
  ///   - Custom setter which writes value to destination
  /// </summary>
  /// <param name="from">Source type</param>
  /// <param name="to">Destination type</param>
  /// <returns></returns>
  IEnumerable<IMappingOperation> GetMappingOperations(Type from, Type to);

  /// <summary>
  /// Gets the root mapping operation.
  /// </summary>
  /// <param name="from">The from.</param>
  /// <param name="to">The to.</param>
  /// <returns>An IRootMappingOperation.</returns>
  IRootMappingOperation GetRootMappingOperation(Type from, Type to);

  /// <summary>
  /// Gets the static converters manager.
  /// </summary>
  /// <returns>A StaticConvertersManager.</returns>
  StaticConvertersManager GetStaticConvertersManager();

  /// <summary>
  ///   Define members which should be ignored
  /// </summary>
  /// <param name="typeFrom">Source type for which ignore members are defining</param>
  /// <param name="typeTo">Destination type for which ignore members are defining</param>
  /// <param name="ignoreNames">Array of member names which should be ignored</param>
  /// <returns></returns>
  IMappingConfigurator IgnoreMembers(Type typeFrom, Type typeTo, string[] ignoreNames);

  /// <summary>
  ///   Define members which should be ignored
  /// </summary>
  /// <typeparam name="TFrom">Source type for which ignore members are defining</typeparam>
  /// <typeparam name="TTo">Destination type for which ignore members are defining</typeparam>
  /// <param name="ignoreNames">Array of member names which should be ignored</param>
  /// <returns></returns>
  IMappingConfigurator IgnoreMembers<TFrom, TTo>(string[] ignoreNames);

  /// <summary>
  ///   Setup function which returns value for destination if appropriate source member is null.
  /// </summary>
  /// <typeparam name="TFrom">Type of source member</typeparam>
  /// <typeparam name="TTo">Type of destination member</typeparam>
  /// <param name="nullSubstitutor">Function which returns value for destination if appropriate source member is null</param>
  /// <returns></returns>
  IMappingConfigurator NullSubstitution<TFrom, TTo>(Func<object, TTo> nullSubstitutor);

  /// <summary>
  ///   Define postprocessor for specified type
  /// </summary>
  /// <typeparam name="T">Objects of this type and all it's descendants will be postprocessed</typeparam>
  /// <param name="postProcessor"></param>
  /// <returns></returns>
  IMappingConfigurator PostProcess<T>(ValuesPostProcessor<T> postProcessor);

  /// <summary>
  ///   Set unique configuration name to force Emit Mapper create new mapper instead using appropriate cached one.
  /// </summary>
  /// <returns></returns>
  IMappingConfigurator SetConfigName(string configurationName);
}