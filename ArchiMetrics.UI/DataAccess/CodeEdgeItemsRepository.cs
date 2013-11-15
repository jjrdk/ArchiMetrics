// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeEdgeItemsRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeEdgeItemsRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.DataAccess
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	public abstract class CodeEdgeItemsRepository : IEdgeItemsRepository, IDisposable
	{
		private static readonly Regex LocRegex = new Regex(@"^(?!(\s*\/\/))\s*.{3,}", RegexOptions.Compiled);
		private readonly ICodeErrorRepository _codeErrorRepository;
		private readonly ConcurrentDictionary<string, Task<IEnumerable<MetricsEdgeItem>>> _edgeItems = new ConcurrentDictionary<string, Task<IEnumerable<MetricsEdgeItem>>>();

		public CodeEdgeItemsRepository(
			ICodeErrorRepository codeErrorRepository)
		{
			_codeErrorRepository = codeErrorRepository;
		}

		~CodeEdgeItemsRepository()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public Task<IEnumerable<MetricsEdgeItem>> GetEdges(string path, bool includeReview, CancellationToken cancellationToken)
		{
			return string.IsNullOrWhiteSpace(path)
					   ? Task.Factory.StartNew(() => new MetricsEdgeItem[0].AsEnumerable(), cancellationToken)
					   : includeReview
							 ? LoadWithCodeReview(path, cancellationToken)
							 : LoadWithoutCodeReview(path);
		}

		protected static MetricsEdgeItem CreateEdgeItem(
			string dependant,
			string dependency,
			string projectPath,
			CodeMetrics dependantMetrics,
			CodeMetrics dependencyMetrics,
			IEnumerable<IGrouping<string, EvaluationResult>> results)
		{
			return new MetricsEdgeItem
			{
				Dependant = dependant,
				Dependency = dependency,
				DependantLinesOfCode = dependantMetrics.LinesOfCode,
				DependantMaintainabilityIndex = dependantMetrics.MaintainabilityIndex,
				DependantComplexity = dependantMetrics.CyclomaticComplexity,
				DependencyLinesOfCode = dependencyMetrics.LinesOfCode,
				DependencyMaintainabilityIndex = dependencyMetrics.MaintainabilityIndex,
				DependencyComplexity = dependencyMetrics.CyclomaticComplexity,
				CodeIssues =
					results.Where(e => e.Key == projectPath)
						.SelectMany(er => er)
						.ToArray()
			};
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				foreach (var task in _edgeItems.Values)
				{
					task.Dispose();
				}

				_edgeItems.Clear();
			}
		}

		protected abstract Task<IEnumerable<MetricsEdgeItem>> CreateEdges(string path, IEnumerable<EvaluationResult> results, CancellationToken cancellationToken);

		protected int GetLinesOfCode(CommonSyntaxNode node)
		{
			return node
					.ToFullString()
					.Split('\n')
					.Count(s => LocRegex.IsMatch(s.Trim()));
		}

		protected IEnumerable<string> GetNamespaceNames(SyntaxNode node)
		{
			return node.DescendantNodesAndSelf()
				.Where(n => n.Kind == SyntaxKind.NamespaceDeclaration)
				.Select(n => ((NamespaceDeclarationSyntax)n).Name.GetText().ToString().Trim());
		}

		protected IEnumerable<string> GetUsings(SyntaxNode node)
		{
			return node.DescendantNodesAndSelf()
					   .Where(n => n.Kind == SyntaxKind.UsingDirective)
					   .Select(n => n as UsingDirectiveSyntax)
					   .Where(n => n != null)
					   .Select(n => n.Name.GetText().ToString().Trim());
		}

		private Task<IEnumerable<MetricsEdgeItem>> LoadWithCodeReview(string path, CancellationToken cancellationToken)
		{
			return _edgeItems.GetOrAdd(
				path + true,
				async p =>
				{
					var errors = await _codeErrorRepository.GetErrors(path, CancellationToken.None);
					var edges = await CreateEdges(path, errors, cancellationToken);

					return edges;
				});
		}

		private Task<IEnumerable<MetricsEdgeItem>> LoadWithoutCodeReview(string path)
		{
			return _edgeItems.GetOrAdd(
				path + false,
				p =>
				{
					_codeErrorRepository.GetErrors(path, CancellationToken.None);
					return CreateEdges(path, new EvaluationResult[0], CancellationToken.None);
				});
		}
	}
}
