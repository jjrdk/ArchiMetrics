// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EdgesViewModelBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EdgesViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Xml.Serialization;
	using ArchiMetrics.Common.Structure;

	public abstract class EdgesViewModelBase : ViewModelBase
	{
		private readonly IAppContext _config;
		private readonly IEdgeTransformer _filter;
		private readonly IEdgeItemsRepository _repository;
		private MetricsEdgeItem[] _allMetricsEdges = new MetricsEdgeItem[0];
		private CancellationTokenSource _tokenSource;
		private ObservableCollection<VertexTransform> _vertexTransforms;

		protected EdgesViewModelBase(
			IEdgeItemsRepository repository, 
			IEdgeTransformer filter, 
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

		public ObservableCollection<VertexTransform> VertexTransforms
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

		protected IEdgeTransformer Filter
		{
			get
			{
				return _filter;
			}
		}

		protected MetricsEdgeItem[] AllMetricsEdges
		{
			get
			{
				return _allMetricsEdges;
			}
		}

		public void SaveTransforms(string filePath)
		{
			using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			{
				var serializer = new XmlSerializer(typeof(List<VertexTransform>));
				serializer.Serialize(stream, VertexTransforms.ToList());
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
			if (forceUpdate || !_allMetricsEdges.Any())
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

				_allMetricsEdges = null;
			}

			base.Dispose(isDisposing);
		}

		private void LoadEdges(CancellationToken cancellationToken)
		{
			IsLoading = true;
			_repository.GetEdges(_config.Path, _config.IncludeCodeReview, cancellationToken)
				.ContinueWith(
					t =>
					{
						_allMetricsEdges = t.Result.ToArray();
						UpdateInternal(cancellationToken);
					}, 
					cancellationToken);
		}
	}
}
