// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vertex.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the Vertex type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using ArchiMeter.Common;

	internal class Vertex : IEquatable<Vertex>
	{
		public Vertex(string name, bool isCircularReference, int complexity, double maintainability, int linesOfCode, IEnumerable<EvaluationResult> evaluationResults = null)
		{
			this.Name = name;
			this.LinesOfCode = linesOfCode;
			this.IsCircularReference = isCircularReference;
			this.EvaluationResults = (evaluationResults ?? new EvaluationResult[0]).ToArray();
			this.Quality = this.EvaluationResults.Any() ? this.EvaluationResults.Select(x => x.Quality).Min() : CodeQuality.Good;
			this.CodeIssues = this.EvaluationResults.Count();
			this.LinesOfCodeWithIssues = this.EvaluationResults.Any() ? this.EvaluationResults.Select(x => x.LinesOfCodeAffected).Sum() : 0;
			this.MaintainabilityIndex = (int)maintainability;
			this.Complexity = complexity;
		}

		public string Name { get; private set; }

		public bool IsCircularReference { get; private set; }

		public int LinesOfCode { get; private set; }

		public int Complexity { get; private set; }

		public int MaintainabilityIndex { get; private set; }

		public CodeQuality Quality { get; private set; }

		public bool ShowMetrics
		{
			get
			{
				return this.LinesOfCode > 0;
			}
		}

		public bool HasIssues
		{
			get { return this.CodeIssues > 0; }
		}

		public int CodeIssues { get; private set; }

		public int LinesOfCodeWithIssues { get; private set; }

		public IEnumerable<EvaluationResult> EvaluationResults { get; private set; }

		public bool Equals(Vertex other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return string.Equals(this.Name, other.Name)
				&& this.CodeIssues.Equals(other.CodeIssues)
				&& this.LinesOfCode.Equals(other.LinesOfCode);
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public static bool operator ==(Vertex left, Vertex right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Vertex left, Vertex right)
		{
			return !Equals(left, right);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (obj.GetType() != this.GetType())
			{
				return false;
			}

			return this.Equals((Vertex)obj);
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}