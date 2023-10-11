namespace EmitMapper.Utils;

public readonly struct ConstructorParameters
{
  public readonly ConstructorInfo Constructor;

  public readonly ParameterInfo[] Parameters;

  /// <summary>
  /// Initializes a new instance of the <see cref="ConstructorParameters"/> class.
  /// </summary>
  /// <param name="constructor">The constructor.</param>
  public ConstructorParameters(ConstructorInfo constructor)
  {
    Constructor = constructor;
    Parameters = constructor.GetParameters();
  }

  /// <summary>
  /// Gets the parameters count.
  /// </summary>
  public int ParametersCount => Parameters.Length;

  /// <summary>
  /// Alls the parameters optional.
  /// </summary>
  /// <returns>A bool.</returns>
  public bool AllParametersOptional()
  {
    return Parameters.All(p => p.IsOptional);
  }
}