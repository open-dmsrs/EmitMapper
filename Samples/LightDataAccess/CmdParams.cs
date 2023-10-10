// ***********************************************************************
// Assembly         : TSharp.Core
// Author           : tangjingbo
// Created          : 08-21-2013
//
// Last Modified By : tangjingbo
// Last Modified On : 08-21-2013
// ***********************************************************************
// <copyright file="CmdParams.cs" company="T#">
//     Copyright (c) T#. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace LightDataAccess;

/// <summary>
///   Class CmdParams
/// </summary>
public class CmdParams : Dictionary<string, object>
{
  /// <summary>
  ///   Initializes a new instance of the <see cref="CmdParams" /> class.
  /// </summary>
  public CmdParams()
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="CmdParams" /> class.
  /// </summary>
  /// <param name="init">The init.</param>
  public CmdParams(Dictionary<string, object> init)
    : base(init)
  {
  }
}