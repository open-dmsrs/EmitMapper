using System;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper;
/// <summary>
/// The emit mapper exception.
/// </summary>

public class EmitMapperException : ApplicationException
{
  public IMappingOperation MappingOperation;

  /// <summary>
  /// Initializes a new instance of the <see cref="EmitMapperException"/> class.
  /// </summary>
  public EmitMapperException()
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="EmitMapperException"/> class.
  /// </summary>
  /// <param name="message">The message.</param>
  public EmitMapperException(string message)
    : base(message)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="EmitMapperException"/> class.
  /// </summary>
  /// <param name="message">The message.</param>
  /// <param name="innerException">The inner exception.</param>
  public EmitMapperException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="EmitMapperException"/> class.
  /// </summary>
  /// <param name="message">The message.</param>
  /// <param name="innerException">The inner exception.</param>
  /// <param name="mappingOperation">The mapping operation.</param>
  public EmitMapperException(string message, Exception innerException, IMappingOperation mappingOperation)
    : base(BuildMessage(message, mappingOperation), innerException)
  {
    MappingOperation = mappingOperation;
  }

  /// <summary>
  /// Builds the message.
  /// </summary>
  /// <param name="message">The message.</param>
  /// <param name="mappingOperation">The mapping operation.</param>
  /// <returns>A string.</returns>
  private static string BuildMessage(string message, IMappingOperation mappingOperation)
  {
    return message + " " + mappingOperation;
  }
}