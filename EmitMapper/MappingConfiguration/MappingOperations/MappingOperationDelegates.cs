namespace EmitMapper.MappingConfiguration.MappingOperations;

/// <summary>
/// </summary>
/// <typeparam name="TResult"></typeparam>
/// <param name="state"></param>
public delegate TResult NullSubstitutor<out TResult>(object state);

/// <summary>
/// </summary>
/// <typeparam name="TResult"></typeparam>
public delegate TResult TargetConstructor<out TResult>();

public delegate TResult ValueConverter<in TValue, out TResult>(TValue value, object state);

public delegate TValue ValuesPostProcessor<TValue>(TValue value, object state);

public delegate bool ValuesFilter<in TValue>(TValue value, object state);