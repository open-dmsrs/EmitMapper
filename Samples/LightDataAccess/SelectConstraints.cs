// ***********************************************************************
// Assembly         : TSharp.Core
// Author           : tangjingbo
// Created          : 08-21-2013
//
// Last Modified By : tangjingbo
// Last Modified On : 08-21-2013
// ***********************************************************************
// <copyright file="SelectConstraints.cs" company="Extendsoft">
//     Copyright (c) Extendsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Linq;

namespace LightDataAccess;

/// <summary>
///     Class FilterConstraints
/// </summary>
public class FilterConstraints
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FilterConstraints" /> class.
    /// </summary>
    public FilterConstraints()
    {
        _Constraints = new List<string>();
        Params = new CmdParams();
    }

    /// <summary>
    ///     Gets or sets the _ constraints.
    /// </summary>
    /// <value>The _ constraints.</value>
    private List<string> _Constraints { get; }

    /// <summary>
    ///     Gets or sets the params.
    /// </summary>
    /// <value>The params.</value>
    public CmdParams Params { get; set; }

    /// <summary>
    ///     Builds the where.
    /// </summary>
    /// <returns>System.String.</returns>
    public string BuildWhere()
    {
        return (_Constraints.Count > 0 ? "WHERE " : "") + _Constraints.Select(c => "(" + c + ")").ToCSV(" AND ");
    }

    /// <summary>
    ///     Adds the specified constraint.
    /// </summary>
    /// <param name="constraint">The constraint.</param>
    public void Add(string constraint)
    {
        Add(constraint, null);
    }

    /// <summary>
    ///     Adds the specified constraint.
    /// </summary>
    /// <param name="constraint">The constraint.</param>
    /// <param name="Params">The params.</param>
    public void Add(string constraint, CmdParams Params)
    {
        _Constraints.Add(constraint);
        if (Params != null)
            foreach (var p in Params)
                this.Params.Add(p.Key, p.Value);
    }
}