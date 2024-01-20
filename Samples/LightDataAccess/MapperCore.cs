// ***********************************************************************
// Assembly         : TSharp.Core
// Author           : tangjingbo
// Created          : 05-23-2013
//
// Last Modified By : tangjingbo
// Last Modified On : 05-23-2013
// ***********************************************************************
// <copyright file="MapperCore.cs" company="T#">
//     Copyright (c) T#. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace LightDataAccess;

/// <summary>
///   The mapper core.
/// </summary>
public class MapperCore
{
	/// <summary>
	///   The default configuration.
	/// </summary>
	private static readonly IMappingConfigurator DefaultConfigurator;

	/// <summary>
	///   The list of mappers.
	/// </summary>
	private static readonly ConcurrentBag<object> Mappers;

	/// <summary>
	///   The list of configurations.
	/// </summary>
	private static readonly ConcurrentBag<Tuple<Type, Type, IMappingConfigurator>> MappingConfigurations;

	/// <summary>
	/// Initializes static members of the <see cref="MapperCore"/> class.
	///   Initializes the <see cref="MapperCore" /> class.
	/// </summary>
	static MapperCore()
	{
		DefaultConfigurator = new DefaultMapConfig();
		Mappers = new ConcurrentBag<object>();
		MappingConfigurations = new ConcurrentBag<Tuple<Type, Type, IMappingConfigurator>>();
	}

	/// <summary>
	///   Gets the configurators.
	/// </summary>
	/// <value>The configurations.</value>
	public virtual Tuple<Type, Type, IMappingConfigurator>[] Configurations => MappingConfigurations.ToArray();

	/// <summary>
	///   Adds the configurator instance.
	/// </summary>
	/// <typeparam name="TFrom">The type of from.</typeparam>
	/// <typeparam name="TTo">The type of to.</typeparam>
	/// <param name="configurator">The configurator.</param>
	public virtual void AddConfiguration<TFrom, TTo>(IMappingConfigurator configurator)
	{
		AssertCore.IsNotNull(configurator, "configurator");

		MappingConfigurations.Add(new Tuple<Type, Type, IMappingConfigurator>(typeof(TFrom), typeof(TTo), configurator));
	}

	/// <summary>
	///   Initializes the mapper.
	/// </summary>
	/// <param name="mapperInitializator">The mapper initialization.</param>
	public void Initialize(IMapperInitializator mapperInitializator)
	{
		mapperInitializator.ConfigureMapper(this);
	}

	/// <summary>
	///   Maps the specified from.
	/// </summary>
	/// <typeparam name="TFrom">The type of from.</typeparam>
	/// <typeparam name="TTo">The type of to.</typeparam>
	/// <param name="from">The object from.</param>
	/// <returns>The mapped object.</returns>
	public virtual TTo Map<TFrom, TTo>(TFrom from)
	{
		AssertCore.ArgumentNotNull(from, "@from");

		var mapper = GetMapper<TFrom, TTo>();

		return mapper.Map(from);
	}

	/// <summary>
	///   Maps the specified from.
	/// </summary>
	/// <typeparam name="TFrom">The type of from.</typeparam>
	/// <typeparam name="TTo">The type of to.</typeparam>
	/// <param name="from">The object from.</param>
	/// <param name="to">The destination object.</param>
	/// <returns>The mapped object.</returns>
	public virtual TTo Map<TFrom, TTo>(TFrom from, TTo to)
	{
		AssertCore.ArgumentNotNull(from, "@from");
		AssertCore.ArgumentNotNull(to, "@to");

		var mapper = GetMapper<TFrom, TTo>();

		return mapper.Map(from, to);
	}

	/// <summary>
	///   Maps the collection.
	/// </summary>
	/// <typeparam name="TFrom">The type of from.</typeparam>
	/// <typeparam name="TTo">The type of to.</typeparam>
	/// <param name="from">The from objects collection.</param>
	/// <returns>The output mapped collection.</returns>
	public virtual IEnumerable<TTo> MapCollection<TFrom, TTo>(IEnumerable<TFrom> from)
	{
		AssertCore.ArgumentNotNullOrEmpty(from, "@from");

		var mapper = GetMapper<TFrom, TTo>();

		return mapper.MapEnum(from);
	}

	/// <summary>
	///   Gets the configuration.
	/// </summary>
	/// <typeparam name="TFrom">The type of from.</typeparam>
	/// <typeparam name="TTo">The type of to.</typeparam>
	/// <returns>The configurator instance.</returns>
	/// NOTE: Resolving from IoC can be added here.
	protected virtual Mapper<TFrom, TTo> GetMapper<TFrom, TTo>()
	{
		var mapper = Mappers.FirstOrDefault(m => m is Mapper<TFrom, TTo>) as Mapper<TFrom, TTo>;

		if (mapper is null)
		{
			var configuration = MappingConfigurations.FirstOrDefault(
			  mp => mp.Item1.IsAssignableFrom(typeof(TFrom)) && mp.Item2.IsAssignableFrom(typeof(TTo)));

			var config = configuration is null ? DefaultConfigurator : configuration.Item3;

			mapper = Mapper.Default.GetMapper<TFrom, TTo>(config);

			Mappers.Add(mapper);
		}

		return mapper;
	}
}