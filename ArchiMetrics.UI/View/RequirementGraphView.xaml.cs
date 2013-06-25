// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphView.xaml.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Interaction logic for GraphView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.View
{
	using System.Windows.Controls;
	using Support;
	using ViewModel;

	/// <summary>
	/// Interaction logic for GraphView.xaml
	/// </summary>
	[DataContext(typeof(RequirementGraphViewModel))]
	public partial class RequirementGraphView : UserControl
	{
		public RequirementGraphView()
		{
			InitializeComponent();
		}
	}
}
