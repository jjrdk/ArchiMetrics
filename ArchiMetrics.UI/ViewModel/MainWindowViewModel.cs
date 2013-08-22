// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   The ViewModel for the application's main window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Structure;

	/// <summary>
	/// The ViewModel for the application's main window.
	/// </summary>
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel(ISolutionEdgeItemsRepositoryConfig config)
			: base(config)
		{
		}
	}
}
