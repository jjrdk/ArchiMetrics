// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeEdgeItemsRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
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
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	public abstract class CodeEdgeItemsRepository : IEdgeItemsRepository, IDisposable
	{
		private static readonly Regex LocRegex = new Regex(@"^(?!(\s*\/\/))\s*.{3,}", RegexOptions.Compiled);
		private readonly ICodeErrorRepository _codeErrorRepository;
		private readonly ISolutionEdgeItemsRepositoryConfig _config;
		private Task<IEnumerable<MetricsEdgeItem>> _edgeItems;

		public CodeEdgeItemsRepository(
			ISolutionEdgeItemsRepositoryConfig config, 
			ICodeErrorRepository codeErrorRepository)
		{
			_config = config;
			_codeErrorRepository = codeErrorRepository;
			_config.PropertyChanged += ConfigPropertyChanged;
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

		public Task<IEnumerable<MetricsEdgeItem>> GetEdgesAsync()
		{
			return _edgeItems ?? (_edgeItems = string.IsNullOrWhiteSpace(_config.Path)
												   ? Task.Factory.StartNew(() => new MetricsEdgeItem[0].AsEnumerable())
												   : _config.IncludeCodeReview
														 ? LoadWithCodeReview()
														 : LoadWithoutCodeReview());
		}

		protected static MetricsEdgeItem CreateEdgeItem(
			string dependant, 
			string dependency, 
			string projectPath, 
			ProjectCodeMetrics dependantMetrics, 
			ProjectCodeMetrics dependencyMetrics, 
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
									  .AsEnumerable()
					   };
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_config.PropertyChanged -= ConfigPropertyChanged;
			}
		}

		protected abstract Task<IEnumerable<MetricsEdgeItem>> CreateEdges(IEnumerable<EvaluationResult> results);

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

		private async Task<IEnumerable<MetricsEdgeItem>> LoadWithCodeReview()
		{
			var errors = await _codeErrorRepository.GetErrorsAsync();
			var edges = await CreateEdges(errors);

			return edges;
		}

		private Task<IEnumerable<MetricsEdgeItem>> LoadWithoutCodeReview()
		{
			return CreateEdges(new EvaluationResult[0]);
		}

		private void ConfigPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_edgeItems != null)
			{
				_edgeItems.Dispose();
			}

			_edgeItems = null;
		}
	}
}
