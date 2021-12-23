// ***********************************************************************
// Assembly         : TSharp.Core
// Author           : tangjingbo
// Created          : 08-21-2013
//
// Last Modified By : tangjingbo
// Last Modified On : 08-21-2013
// ***********************************************************************
// <copyright file="ThreadConnection.cs" company="T#">
//     Copyright (c) T#. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Data;
using System.Data.Common;

namespace LightDataAccess;

/// <summary>
///     Class ThreadConnection
/// </summary>
public class ThreadConnection : IDisposable
{
    /// <summary>
    ///     The connection
    /// </summary>
    [ThreadStatic] private static DbConnection _connection;

    /// <summary>
    ///     The entries count
    /// </summary>
    [ThreadStatic] private static int _entriesCount;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ThreadConnection" /> class.
    /// </summary>
    /// <param name="connectionCreator">The connection creator.</param>
    public ThreadConnection(Func<DbConnection> connectionCreator)
    {
        if (_connection == null || _connection.State == ConnectionState.Broken) _connection = connectionCreator();
        _entriesCount++;
    }

    /// <summary>
    ///     Gets the connection.
    /// </summary>
    /// <value>The connection.</value>
    public DbConnection Connection
    {
        get
        {
            if (_connection.State == ConnectionState.Closed) _connection.Open();
            return _connection;
        }
    }

    #region IDisposable Members

    /// <summary>
    ///     执行与释放或重置非托管资源相关的应用程序定义的任务。
    /// </summary>
    public void Dispose()
    {
        if (_entriesCount <= 1)
        {
            if (_connection != null)
            {
                using (_connection)
                {
                    _connection.Close();
                }
            }

            _connection = null;
            _entriesCount = 0;
        }
        else
        {
            _entriesCount--;
        }
    }

    #endregion
}