// ***********************************************************************
// Assembly         : TSharp.Core
// Author           : tangjingbo
// Created          : 08-21-2013
//
// Last Modified By : tangjingbo
// Last Modified On : 08-21-2013
// ***********************************************************************
// <copyright file="DbSettings.cs" company="Extendsoft">
//     Copyright (c) Extendsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace LightDataAccess
{
    /// <summary>
    /// Class DbSettings
    /// </summary>
	public class DbSettings
    {
        /// <summary>
        /// The first name escape symbol
        /// </summary>
		public string firstNameEscapeSymbol;
        /// <summary>
        /// The second name escape symbol
        /// </summary>
		public string secondNameEscapeSymbol;
        /// <summary>
        /// The param prefix
        /// </summary>
		public string paramPrefix;

        /// <summary>
        /// My SQL
        /// </summary>
		public static DbSettings MySQL;
        /// <summary>
        /// The MSSQL
        /// </summary>
		public static DbSettings MSSQL;

        /// <summary>
        /// The SQLITE
        /// </summary>
        public static DbSettings SQLITE;

        /// <summary>
        /// Initializes static members of the <see cref="DbSettings"/> class.
        /// </summary>
		static DbSettings()
        {
            MySQL = new DbSettings
            {
                firstNameEscapeSymbol = "`",
                secondNameEscapeSymbol = "`",
                paramPrefix = "@p_"
            };

            MSSQL = new DbSettings
            {
                firstNameEscapeSymbol = "[",
                secondNameEscapeSymbol = "]",
                paramPrefix = "@p_"
            };
            SQLITE = new DbSettings
            {
                firstNameEscapeSymbol = "\"",
                secondNameEscapeSymbol = "\"",
                paramPrefix = "@p_"
            };
        }

        /// <summary>
        /// Gets the name of the param.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>System.String.</returns>
		public string GetParamName(string fieldName)
        {
            return paramPrefix + fieldName;
        }

        /// <summary>
        /// Gets the name of the escaped.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
		public string GetEscapedName(string name)
        {
            return firstNameEscapeSymbol + name + secondNameEscapeSymbol;
        }

    }
}
