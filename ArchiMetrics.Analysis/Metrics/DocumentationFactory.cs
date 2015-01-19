namespace ArchiMetrics.Analysis.Metrics
{
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Xml.Linq;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;
	using Microsoft.CodeAnalysis;

	internal class DocumentationFactory : IAsyncFactory<ISymbol, IDocumentation>
	{
		/// <summary>
		/// Creates the requested instance as an asynchronous operation.
		/// </summary>
		/// <param name="parameter">The parameter to pass to the object creation.</param>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/> to use for cancelling the object creation.</param>
		/// <returns>Returns a <see cref="Task{T}"/> which represents the instance creation task.</returns>
		public Task<IDocumentation> Create(ISymbol parameter, CancellationToken cancellationToken)
		{
			var doc = parameter.GetDocumentationCommentXml();
			if (string.IsNullOrWhiteSpace(doc))
			{
				return Task.FromResult<IDocumentation>(null);
			}

			var xmldoc = XDocument.Parse(doc);
			var docRoot = xmldoc.Root;
			if (docRoot == null)
			{
				return Task.FromResult<IDocumentation>(null);
			}

			var summaryElement = docRoot.Element("summary");
			var summary = summaryElement == null ? string.Empty : summaryElement.Value.Trim();
			var codeElement = docRoot.Element("code");
			var code = codeElement == null ? string.Empty : codeElement.Value.Trim();
			var exampleElement = docRoot.Element("example");
			var example = exampleElement == null ? string.Empty : exampleElement.Value.Trim();
			var remarksElement = docRoot.Element("remarks");
			var remarks = remarksElement == null ? string.Empty : remarksElement.Value.Trim();
			var returnsElement = docRoot.Element("returns");
			var returns = returnsElement == null ? string.Empty : returnsElement.Value.Trim();
			var exceptionElements = docRoot.Elements("exception");
			var exceptions = exceptionElements.Select(x => new ExceptionDescription(x.Attribute("cref").Value.Trim(), x.Value.Trim()));

			IDocumentation documentation = new Documentation(summary, code, example, remarks, returns, exceptions);

			return Task.FromResult(documentation);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}
	}
}