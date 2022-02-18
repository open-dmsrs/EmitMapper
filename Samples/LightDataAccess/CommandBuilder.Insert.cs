// ***********************************************************************
// Assembly         : TSharp.Core
// Author           : tangjingbo
// Created          : 08-21-2013
//
// Last Modified By : tangjingbo
// Last Modified On : 08-21-2013
// ***********************************************************************
// <copyright file="CommandBuilder.Insert.cs" company="T#">
//     Copyright (c) T#. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Data;
using System.Data.Common;
using System.Linq;
using EmitMapper;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using LightDataAccess.MappingConfigs;

namespace LightDataAccess;

/// <summary>
///   Class CommandBuilder
/// </summary>
public static partial class CommandBuilder
{
  /// <summary>
  ///   Builds the insert command.
  /// </summary>
  /// <param name="cmd">The CMD.</param>
  /// <param name="obj">The obj.</param>
  /// <param name="tableName">Name of the table.</param>
  /// <param name="dbSettings">The db settings.</param>
  /// <returns>DbCommand.</returns>
  public static DbCommand BuildInsertCommand(this DbCommand cmd, object obj, string tableName, DbSettings dbSettings)
  {
    return BuildInsertCommand(cmd, obj, tableName, dbSettings, null, null);
  }

  /// <summary>
  ///   Builds the insert command.
  /// </summary>
  /// <param name="cmd">The CMD.</param>
  /// <param name="obj">The obj.</param>
  /// <param name="tableName">Name of the table.</param>
  /// <param name="dbSettings">The db settings.</param>
  /// <param name="includeFields">The include fields.</param>
  /// <param name="excludeFields">The exclude fields.</param>
  /// <returns>DbCommand.</returns>
  public static DbCommand BuildInsertCommand(
    this DbCommand cmd,
    object obj,
    string tableName,
    DbSettings dbSettings,
    string[] includeFields,
    string[] excludeFields)
  {
    IMappingConfigurator config = new AddDbCommandsMappingConfig(
      dbSettings,
      includeFields,
      excludeFields,
      "insertop_inc_" + includeFields.ToCsv("_") + "_exc_" + excludeFields.ToCsv("_"));

    var mapper = ObjectMapperManager.DefaultInstance.GetMapperImpl(obj.GetType(), typeof(DbCommand), config);

    var fields = mapper.StoredObjects.OfType<SrcReadOperation>().Select(m => m.Source.MemberInfo.Name);

    var cmdStr = "INSERT INTO " + tableName + "(" + fields.Select(dbSettings.GetEscapedName).ToCsv(",") + ") VALUES ("
                 + fields.Select(dbSettings.GetParamName).ToCsv(",") + ")";
    cmd.CommandText = cmdStr;
    cmd.CommandType = CommandType.Text;

    mapper.Map(obj, cmd, null);
    return cmd;
  }
}