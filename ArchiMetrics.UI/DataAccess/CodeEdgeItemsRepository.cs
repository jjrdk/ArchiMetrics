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
	using ArchiMetrics.Common.Metrics;

	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	public abstract class CodeEdgeItemsRepository : IEdgeItemsRepository, IDisposable
	{
		private static readonly Regex LocRegex = new Regex(@"^(?!(\s*\/\/))\s*.{3,}", RegexOptions.Compiled);
		private readonly ICodeErrorRepository _codeErrorRepository;
		private readonly ISolutionEdgeItemsRepositoryConfig _config;
		private Task<IEnumerable<EdgeItem>> _edgeItems;

		public CodeEdgeItemsRepository(
			ISolutionEdgeItemsRepositoryConfig config, 
			ICodeErrorRepository codeErrorRepository)
		{
			this._config = config;
			this._codeErrorRepository = codeErrorRepository;
			this._config.PropertyChanged += this.ConfigPropertyChanged;
		}

		~CodeEdgeItemsRepository()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public Task<IEnumerable<EdgeItem>> GetEdgesAsync()
		{
			return this._edgeItems ?? (this._edgeItems = string.IsNullOrWhiteSpace(this._config.Path)
												   ? Task.Factory.StartNew(() => new EdgeItem[0].AsEnumerable())
												   : this._config.IncludeCodeReview
														 ? this.LoadWithCodeReview()
														 : this.LoadWithoutCodeReview());
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this._config.PropertyChanged -= this.ConfigPropertyChanged;
			}
		}

		protected abstract Task<IEnumerable<EdgeItem>> CreateEdges(IEnumerable<EvaluationResult> results);

		protected static EdgeItem CreateEdgeItem(
			string dependant, 
			string dependency, 
			string projectPath, 
			ProjectCodeMetrics dependantMetrics, 
			ProjectCodeMetrics dependencyMetrics, 
			IEnumerable<IGrouping<string, EvaluationResult>> results)
		{
			return new EdgeItem
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

		private async Task<IEnumerable<EdgeItem>> LoadWithCodeReview()
		{
			var errors = await this._codeErrorRepository.GetErrorsAsync();
			var edges = await this.CreateEdges(errors);

			return edges;
		}

		private Task<IEnumerable<EdgeItem>> LoadWithoutCodeReview()
		{
			return this.CreateEdges(new EvaluationResult[0]);
		}

		private void ConfigPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this._edgeItems != null)
			{
				this._edgeItems.Dispose();
			}

			this._edgeItems = null;
		}
	}
}
