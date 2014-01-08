// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackingCodeErrorRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TrackingCodeErrorRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.DataAccess
{
	using System.ComponentModel;
	using ArchiMetrics.Analysis.Model;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Structure;
	using Roslyn.Services;

	internal class TrackingCodeErrorRepository : CodeErrorRepository
	{
		private readonly IAppContext _config;

		public TrackingCodeErrorRepository(IAppContext config, IProvider<string, ISolution> solutionProvider, INodeInspector inspector, IAvailableRules availableRules)
			: base(solutionProvider, inspector, availableRules)
		{
			_config = config;
			_config.PropertyChanged += ConfigPropertyChanged;
			GetErrors(_config.Path);
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_config.PropertyChanged -= ConfigPropertyChanged;
			}

			base.Dispose(isDisposing);
		}

		private void ConfigPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "Path")
			{
				return;
			}

			GetErrors(_config.Path);
		}
	}
}
