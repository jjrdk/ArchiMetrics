// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureRulesViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the StructureRulesViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.ComponentModel;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Windows.Input;
	using ArchiMetrics.Analysis.Validation;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Structure;
	using ArchiMetrics.UI.Support;
	using Newtonsoft.Json;

	internal class StructureRulesViewModel : ViewModelBase
	{
		private readonly IAppContext _config;
		private readonly IProvider<string, ObservableCollection<TransformRule>> _transformRulesProvider;
		private readonly IModelValidator _modelValidator;
		private IEnumerable<IValidationResult> _validation;

		public StructureRulesViewModel(
			IAppContext config,
			IProvider<string, ObservableCollection<TransformRule>> transformRulesProvider,
			IModelValidator modelValidator)
			: base(config)
		{
			_config = config;
			_transformRulesProvider = transformRulesProvider;
			_modelValidator = modelValidator;
			GraphToVisualize = new ModelGraph(Enumerable.Empty<ModelEdge>());
			ModelRules = new ObservableCollection<IModelRule>();
		}

		public string TransformRuleSource
		{
			get
			{
				return _config.RulesSource;
			}

			set
			{
				if (!_config.RulesSource.Equals(value))
				{
					_config.RulesSource = value;
					RaisePropertyChanged();
					RaisePropertyChanged(new PropertyChangedEventArgs("VertexTransforms"));
				}
			}
		}

		public ObservableCollection<IModelRule> ModelRules { get; private set; }

		public IEnumerable<TransformRule> VertexTransforms
		{
			get
			{
				return _transformRulesProvider.Get(_config.RulesSource);
			}
		}

		public ModelGraph GraphToVisualize { get; private set; }

		public ICommand AddItemsCommand { get; private set; }

		public ICommand DeleteItemsCommand { get; private set; }

		public ICommand LoadModelCommand { get; private set; }

		public ICommand SaveModelCommand { get; private set; }

		public void SaveTransforms(string filePath)
		{
			using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			using (var writer = new StreamWriter(stream))
			{
				var json = JsonConvert.SerializeObject(VertexTransforms.ToList());
				writer.Write(json);
			}
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
			}

			base.Dispose(isDisposing);
		}

		private async void ValidateModel()
		{
			IsLoading = true;
			var rules = _transformRulesProvider.Get(_config.RulesSource);
			var validation = (await _modelValidator.Validate(_config.Path, ModelRules, rules, CancellationToken.None)).ToArray();
			var passed = validation.Where(x => x.Passed);
			var notPassed = validation.WhereNot(x => x.Passed);
			IsLoading = false;
		}
	}
}
