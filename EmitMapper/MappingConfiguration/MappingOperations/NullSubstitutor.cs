namespace EmitMapper.MappingConfiguration.MappingOperations;

/// <summary>
/// </summary>
/// <typeparam name="TResult"></typeparam>
/// <param name="state"></param>
public delegate TResult NullSubstitutor<out TResult>(object state);