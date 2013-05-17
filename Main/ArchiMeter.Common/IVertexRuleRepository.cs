// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVertexRuleRepository.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IVertexRuleRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common
{
	using System;
	using System.Collections.Generic;

	public interface IVertexRuleRepository : IVertexRuleDefinition
	{
		IEnumerable<Func<string, string>> GetAllVertexPreTransforms();

		IEnumerable<Func<string, string>> GetAllVertexPostTransforms();
	}
}