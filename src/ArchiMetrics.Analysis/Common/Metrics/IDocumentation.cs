// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDocumentation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
    public interface IDocumentation
    {
        string Summary { get; }

        string Returns { get; }

        string Code { get; }

        string Example { get; }

        string Remarks { get; }
    }
}