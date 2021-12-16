using System;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper;

public class EmitMapperException : ApplicationException
{
    public IMappingOperation MappingOperation;

    public EmitMapperException()
    {
    }

    public EmitMapperException(string message)
        : base(message)
    {
    }

    public EmitMapperException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public EmitMapperException(string message, Exception innerException, IMappingOperation mappingOperation)
        : base(BuildMessage(message, mappingOperation), innerException)
    {
        MappingOperation = mappingOperation;
    }

    private static string BuildMessage(string message, IMappingOperation mappingOperation)
    {
        return message + " " + mappingOperation;
    }
}