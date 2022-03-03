namespace EmitMapper.MappingConfiguration.MappingOperations;

public delegate TResult ValueConverter<in TValue, out TResult>(TValue value, object state);