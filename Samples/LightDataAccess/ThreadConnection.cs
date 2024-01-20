// *********************************************************************** Assembly : TSharp.Core Author : tangjingbo Created : 08-21-2013
//
// Last Modified By : tangjingbo Last Modified On : 08-21-2013 ***********************************************************************
// <copyright file="ThreadConnection.cs" company="T#">
//     Copyright (c) T#. All rights reserved.
// </copyright>
// <summary>
// </summary>
// ***********************************************************************

namespace LightDataAccess;

/// <summary>
/// Class ThreadConnection.
/// </summary>
public class ThreadConnection : IDisposable
{
	/// <summary>
	/// The connection.
	/// </summary>
	[ThreadStatic] private static DbConnection connection;

	/// <summary>
	/// The entries count.
	/// </summary>
	[ThreadStatic] private static int entriesCount;

	/// <summary>
	/// Initializes a new instance of the <see cref="ThreadConnection"/> class.
	/// </summary>
	/// <param name="connectionCreator"> The connection creator. </param>
	public ThreadConnection(Func<DbConnection> connectionCreator)
	{
		if (connection is null || connection.State == ConnectionState.Broken)
		{
			connection = connectionCreator();
		}

		entriesCount++;
	}

	/// <summary>
	/// Gets the connection.
	/// </summary>
	/// <value> The connection. </value>
	public DbConnection Connection
	{
		get
		{
			if (connection.State == ConnectionState.Closed)
			{
				connection.Open();
			}

			return connection;
		}
	}

	/// <summary>
	/// 执行与释放或重置非托管资源相关的应用程序定义的任务。.
	/// </summary>
	public void Dispose()
	{
		if (entriesCount <= 1)
		{
			if (connection != null)
			{
				using (connection)
				{
				}
			}

			connection = null;
			entriesCount = 0;
		}
		else
		{
			entriesCount--;
		}
	}
}