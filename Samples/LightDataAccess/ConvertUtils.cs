// ***********************************************************************
// Assembly         : TSharp.Core
// Author           : tangjingbo
// Created          : 08-21-2013
//
// Last Modified By : tangjingbo
// Last Modified On : 08-21-2013
// ***********************************************************************
// <copyright file="ConvertUtils.cs" company="T#">
//     Copyright (c) T#. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace LightDataAccess;

using System;

/// <summary>
///   Class ConvertUtils
/// </summary>
public static class ConvertUtils
{
  /// <summary>
  ///   To the bool.
  /// </summary>
  /// <param name="s">The s.</param>
  /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
  public static bool ToBool(this short s)
  {
    return s != 0;
  }

  /// <summary>
  ///   To the bool.
  /// </summary>
  /// <param name="s">The s.</param>
  /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
  public static bool ToBool(this short? s)
  {
    return s != 0;
  }

  /// <summary>
  ///   To the GUID.
  /// </summary>
  /// <param name="str">The STR.</param>
  /// <returns>Guid.</returns>
  public static Guid ToGuid(this string str)
  {
    if (str == null) return Guid.Empty;
    return new Guid(str);
  }

  /// <summary>
  ///   To the GUID STR.
  /// </summary>
  /// <param name="str">The STR.</param>
  /// <returns>System.String.</returns>
  public static string ToGuidStr(this string str)
  {
    if (str == null) return null;
    return str.ToUpper();
  }

  /// <summary>
  ///   To the GUID STR.
  /// </summary>
  /// <param name="guid">The GUID.</param>
  /// <returns>System.String.</returns>
  public static string ToGuidStr(this Guid guid)
  {
    return guid.ToString().ToUpper();
  }

  /// <summary>
  ///   To the GUID STR.
  /// </summary>
  /// <param name="guid">The GUID.</param>
  /// <returns>System.String.</returns>
  public static string ToGuidStr(this Guid? guid)
  {
    if (guid == null) return null;
    return guid.Value.ToString().ToUpper();
  }

  /// <summary>
  ///   To the short.
  /// </summary>
  /// <param name="b">if set to <c>true</c> [b].</param>
  /// <returns>System.Int16.</returns>
  public static short ToShort(this bool b)
  {
    return b ? (short)1 : (short)0;
  }
}