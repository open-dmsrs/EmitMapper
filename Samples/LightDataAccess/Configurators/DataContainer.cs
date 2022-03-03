using System.Collections.Generic;

namespace LightDataAccess.Configurators;

/// <summary>
///   The data container.
/// </summary>
internal class DataContainer
{
  /// <summary>
  ///   Gets the fields.
  /// </summary>
  public Dictionary<string, string> Fields { get; internal set; }
}