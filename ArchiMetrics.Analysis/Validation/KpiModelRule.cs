// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KpiModelRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the KpiModelRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Validation
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.Structure;

	internal class KpiModelRule : IModelRule
	{
		public Task<IEnumerable<IValidationResult>> Validate(IModelNode modelTree)
		{
			return
				Task.Factory.StartNew(
					() => modelTree.Children.SelectMany(x => x.Flatten())
							  .Where(x => x.Type == NodeKind.Class)
							  .Where(x => x.CyclomaticComplexity > 30 || x.MaintainabilityIndex < 40 || x.LinesOfCode > 30)
							  .Select(x => new KpiResult(false, x))
							  .Cast<IValidationResult>()
							  .ToArray()
							  .AsEnumerable());
		}
	}
}