// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VertexViewModelBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the VertexViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.ObjectModel;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using ArchiMetrics.Common.Structure;
	using Newtonsoft.Json;

	internal abstract class VertexViewModelBase : ViewModelBase
	{
		private readonly IAppContext _config;
		private readonly ISyntaxTransformer _filter;
		private readonly IVertexRepository _repository;
		private IModelNode[] _allVertices = new IModelNode[0];
		private CancellationTokenSource _tokenSource;
		private ObservableCollection<TransformRule> _vertexTransforms;

		protected VertexViewModelBase(
			IVertexRepository repository, 
			ISyntaxTransformer filter, 
			IAppContext config)
			: base(config)
		{
			_repository = repository;
			_filter = filter;
			_config = config;
		}

		public string VertexRules
		{
			get
			{
				return _config.RulesSource;
			}

			set
			{
				_config.RulesSource = value;
				RaisePropertyChanged();
			}
		}

		public ObservableCollection<TransformRule> VertexTransforms
		{
			get
			{
				return _vertexTransforms;
			}

			set
			{
				if (!ReferenceEquals(_vertexTransforms, value))
				{
					_vertexTransforms = value;
					RaisePropertyChanged();
				}
			}
		}

		protected ISyntaxTransformer Filter
		{
			get
			{
				return _filter;
			}
		}

		protected IModelNode[] AllVertices
		{
			get
			{
				return _allVertices;
			}
		}

		public void SaveTransforms(string filePath)
		{
			var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
			using (var writer = new StreamWriter(stream))
			{
				var json = JsonConvert.SerializeObject(VertexTransforms.ToList());
				writer.Write(json);
			}
		}

		protected override void Update(bool forceUpdate)
		{
			UpdateImpl(forceUpdate);
		}

		protected void UpdateImpl(bool forceUpdate)
		{
			if (_tokenSource != null)
			{
				_tokenSource.Cancel(false);
				_tokenSource.Dispose();
			}

			_tokenSource = new CancellationTokenSource();
			base.Update(forceUpdate);
			if (forceUpdate || !_allVertices.Any())
			{
				LoadEdges(_tokenSource.Token);
			}
			else
			{
				UpdateInternal(_tokenSource.Token);
			}
		}

		protected abstract void UpdateInternal(CancellationToken cancellationToken);

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				if (_tokenSource != null)
				{
					_tokenSource.Dispose();
				}

				_allVertices = null;
			}

			base.Dispose(isDisposing);
		}

		private async void LoadEdges(CancellationToken cancellationToken)
		{
			IsLoading = true;
			var edges = await _repository.GetVertices(_config.Path, cancellationToken).ConfigureAwait(false);

			_allVertices = edges.ToArray();
			UpdateInternal(cancellationToken);
		}
	}
}
