// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EdgeItemBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EdgeItemBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ArchiMetrics.Common.Structure
{
	[Serializable]
    public class EdgeItemBase
    {
        public string Dependant { get; set; }

        public string Dependency { get; set; }

        public int MergedEdges { get; set; }

        public override string ToString()
        {
            return string.Format("({0} -> {1})", Dependant, Dependency);
        }
    }
}