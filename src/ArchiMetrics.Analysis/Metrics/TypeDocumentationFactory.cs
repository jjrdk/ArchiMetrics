// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeDocumentationFactory.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeDocumentationFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Xml.Linq;
	using Common;
	using Common.Metrics;
	using Microsoft.CodeAnalysis;

	internal class TypeDocumentationFactory : IAsyncFactory<ISymbol, ITypeDocumentation>
	{
		/// <summary>
		/// Creates the requested instance as an asynchronous operation.
		/// </summary>
		/// <param name="memberSymbol">The memberSymbol to pass to the object creation.</param>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/> to use for cancelling the object creation.</param>
		/// <returns>Returns a <see cref="Task{T}"/> which represents the instance creation task.</returns>
		public Task<ITypeDocumentation> Create(ISymbol memberSymbol, CancellationToken cancellationToken)
		{
			var doc = memberSymbol.GetDocumentationCommentXml();
			if (string.IsNullOrWhiteSpace(doc))
			{
				return Task.FromResult<ITypeDocumentation>(null);
			}

			var xmldoc = XDocument.Parse(doc);
			var docRoot = xmldoc.Root;
			if (docRoot == null)
			{
				return Task.FromResult<ITypeDocumentation>(null);
			}

			var summaryElement = docRoot.Element("summary");
			var summary = summaryElement == null ? string.Empty : summaryElement.Value.Trim();
			var codeElement = docRoot.Element("code");
			var code = codeElement == null ? string.Empty : codeElement.Value.Trim();
			var exampleElement = docRoot.Element("example");
			var example = exampleElement == null ? string.Empty : exampleElement.Value.Trim();
			var remarksElement = docRoot.Element("remarks");
			var remarks = remarksElement?.Value.Trim() ?? string.Empty;
			var returnsElement = docRoot.Element("returns");
			var returns = returnsElement == null ? string.Empty : returnsElement.Value.Trim();
			var typeParameterElements = docRoot.Elements("typeparam");
			var typeConstraints = GetTypeContraints(memberSymbol);
			var typeParameters =
				typeParameterElements.Select(
					x =>
						{
							var name = x.Attribute("name").Value.Trim();
							return new TypeParameterDocumentation(
								name,
								typeConstraints.ContainsKey(name) ? typeConstraints[name] : null,
								x.Value.Trim());
						});

			var documentation = new TypeDocumentation(summary, code, example, remarks, returns, typeParameters);

			return Task.FromResult<ITypeDocumentation>(documentation);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}

		private static IDictionary<string, string> GetTypeContraints(ISymbol symbol)
		{
			var method = symbol as IMethodSymbol;
			if (method == null)
			{
				return new Dictionary<string, string>();
			}

			var enumerable = method.TypeParameters.Select(CreateTypeConstraint);
			IDictionary<string, string> typeParameterConstraints = enumerable.ToDictionary(_ => _.Key, _ => _.Value);

			return typeParameterConstraints;
		}

		private static KeyValuePair<string, string> CreateTypeConstraint(ITypeParameterSymbol typeParameter)
		{
			return new KeyValuePair<string, string>(typeParameter.Name, typeParameter.ToDisplayString());
		}
	}
}