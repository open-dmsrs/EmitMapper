// ***********************************************************************
// Assembly         : TSharp.Core
// Author           : tangjingbo
// Created          : 08-21-2013
//
// Last Modified By : tangjingbo
// Last Modified On : 08-21-2013
// ***********************************************************************
// <copyright file="DbSettings.cs" company="T#">
//     Copyright (c) T#. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace LightDataAccess;

/// <summary>
///   Class DbSettings.
/// </summary>
public class DbSettings
{
  /// <summary>
  ///   The MSSQL.
  /// </summary>
  public static DbSettings Mssql;

  /// <summary>
  ///   My SQL.
  /// </summary>
  public static DbSettings MySql;

  /// <summary>
  ///   The SQLITE.
  /// </summary>
  public static DbSettings Sqlite;

  /// <summary>
  ///   The first name escape symbol.
  /// </summary>
  public string FirstNameEscapeSymbol;

  /// <summary>
  ///   The param prefix.
  /// </summary>
  public string ParamPrefix;

  /// <summary>
  ///   The second name escape symbol.
  /// </summary>
  public string SecondNameEscapeSymbol;

  /// <summary>
  ///   Initializes static members of the <see cref="DbSettings" /> class.
  /// </summary>
  static DbSettings()
  {
    MySql = new DbSettings { FirstNameEscapeSymbol = "`", SecondNameEscapeSymbol = "`", ParamPrefix = "@p_" };

    Mssql = new DbSettings { FirstNameEscapeSymbol = "[", SecondNameEscapeSymbol = "]", ParamPrefix = "@p_" };
    Sqlite = new DbSettings { FirstNameEscapeSymbol = "\"", SecondNameEscapeSymbol = "\"", ParamPrefix = "@p_" };
  }

  /// <summary>
  ///   Gets the name of the escaped.
  /// </summary>
  /// <param name="name">The name.</param>
  /// <returns>System.String.</returns>
  public string GetEscapedName(string name)
  {
    return FirstNameEscapeSymbol + name + SecondNameEscapeSymbol;
  }

  /// <summary>
  ///   Gets the name of the param.
  /// </summary>
  /// <param name="fieldName">Name of the field.</param>
  /// <returns>System.String.</returns>
  public string GetParamName(string fieldName)
  {
    return ParamPrefix + fieldName;
  }
}