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
	private static readonly IMappingConfigurator _DefaultConfigurator;

	/// <summary>
	///   The list of mappers.
	/// </summary>
	private static readonly ConcurrentBag<object> _Mappers;

	/// <summary>
	///   The list of configurations.
	/// </summary>
	private static readonly ConcurrentBag<Tuple<Type, Type, IMappingConfigurator>> _MappingConfigurations;

	/// <summary>
	///   Initializes the <see cref="MapperCore" /> class.
	/// </summary>
	static MapperCore()
	{
		_DefaultConfigurator = new DefaultMapConfig();
		_Mappers = new ConcurrentBag<object>();
		_MappingConfigurations = new ConcurrentBag<Tuple<Type, Type, IMappingConfigurator>>();
	}

	/// <summary>
	///   Gets the configurators.
	/// </summary>
	/// <value>The configurations.</value>
	public virtual Tuple<Type, Type, IMappingConfigurator>[] Configurations => _MappingConfigurations.ToArray();

	/// <summary>
	///   Adds the configurator instance.
	/// </summary>
	/// <typeparam name="TFrom">The type of from.</typeparam>
	/// <typeparam name="TTo">The type of to.</typeparam>
	/// <param name="configurator">The configurator.</param>
	public virtual void AddConfiguration<TFrom, TTo>(IMappingConfigurator configurator)
	{
		AssertCore.IsNotNull(configurator, "configurator");

		_MappingConfigurations.Add(new Tuple<Type, Type, IMappingConfigurator>(typeof(TFrom), typeof(TTo), configurator));
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
		var mapper = _Mappers.FirstOrDefault(m => m is Mapper<TFrom, TTo>) as Mapper<TFrom, TTo>;

		if (mapper is null)
		{
			var configuration = _MappingConfigurations.FirstOrDefault(
			  mp => mp.Item1.IsAssignableFrom(typeof(TFrom)) && mp.Item2.IsAssignableFrom(typeof(TTo)));

			var config = configuration is null ? _DefaultConfigurator : configuration.Item3;

			mapper = Mapper.Default.GetMapper<TFrom, TTo>(config);

			_Mappers.Add(mapper);
		}

		return mapper;
	}
}