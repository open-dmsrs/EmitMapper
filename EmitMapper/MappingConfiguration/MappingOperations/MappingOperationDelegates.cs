namespace EmitMapper.MappingConfiguration.MappingOperations;

public delegate TResult NullSubstitutor<out TResult>(object state);

public delegate TResult TargetConstructor<out TResult>();

public delegate TResult ValueConverter<in TValue, out TResult>(TValue value, object state);

public delegate TValue ValuesPostProcessor<TValue>(TValue value, object state);

public delegate bool ValuesFilter<in TValue>(TValue value, object state);