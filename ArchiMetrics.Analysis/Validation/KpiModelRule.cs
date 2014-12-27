// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KpiModelRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
	using ArchiMetrics.Analysis.Model;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Structure;

	internal class KpiModelRule : IModelRule
	{
		private readonly int _cyclomaticComplexity;
		private readonly int _linesOfCode;
		private readonly double _maintainabilityIndex;

		public KpiModelRule(int cyclomaticComplexity, double maintainabilityIndex, int linesOfCode)
		{
			_cyclomaticComplexity = cyclomaticComplexity;
			_maintainabilityIndex = maintainabilityIndex;
			_linesOfCode = linesOfCode;
		}

		public KpiModelRule()
			: this(30, 40, 30)
		{
		}

		public Task<IEnumerable<IValidationResult>> Validate(IModelNode modelTree)
		{
			return
				Task.Factory.StartNew(
					() => modelTree.Children.SelectMany(x => x.Flatten())
							  .Where(x => x.Type == NodeKind.Class)
							  .Where(x => x.CyclomaticComplexity > _cyclomaticComplexity || x.MaintainabilityIndex < _maintainabilityIndex || x.LinesOfCode > _linesOfCode)
							  .Select(x => new KpiResult(false, x))
							  .Cast<IValidationResult>()
							  .AsArray()
							  .AsEnumerable());
		}
	}
}