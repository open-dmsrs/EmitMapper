namespace EmitMapper.MappingConfiguration.MappingOperations;

public delegate bool ValuesFilter<in TValue>(TValue value, object state);