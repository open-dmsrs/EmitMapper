// ***********************************************************************
// Assembly         : TSharp.Core
// Author           : tangjingbo
// Created          : 08-21-2013
//
// Last Modified By : tangjingbo
// Last Modified On : 08-21-2013
// ***********************************************************************
// <copyright file="SelectConstraints.cs" company="T#">
//     Copyright (c) T#. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Linq;

namespace LightDataAccess;

/// <summary>
///   Class FilterConstraints
/// </summary>
public class FilterConstraints
{
  /// <summary>
  ///   Initializes a new instance of the <see cref="FilterConstraints" /> class.
  /// </summary>
  public FilterConstraints()
  {
    Constraints = new List<string>();
    Params = new CmdParams();
  }

  /// <summary>
  ///   Gets or sets the params.
  /// </summary>
  /// <value>The params.</value>
  public CmdParams Params { get; set; }

  /// <summary>
  ///   Gets or sets the _ constraints.
  /// </summary>
  /// <value>The _ constraints.</value>
  private List<string> Constraints { get; }

  /// <summary>
  ///   Adds the specified constraint.
  /// </summary>
  /// <param name="constraint">The constraint.</param>
  public void Add(string constraint)
  {
    Add(constraint, null);
  }

  /// <summary>
  ///   Adds the specified constraint.
  /// </summary>
  /// <param name="constraint">The constraint.</param>
  /// <param name="params">The params.</param>
  public void Add(string constraint, CmdParams @params)
  {
    Constraints.Add(constraint);
    if (@params != null)
      foreach (var p in @params)
        Params.Add(p.Key, p.Value);
  }

  /// <summary>
  ///   Builds the where.
  /// </summary>
  /// <returns>System.String.</returns>
  public string BuildWhere()
  {
    return (Constraints.Count > 0 ? "WHERE " : string.Empty) + Constraints.Select(c => "(" + c + ")").ToCsv(" AND ");
  }
}