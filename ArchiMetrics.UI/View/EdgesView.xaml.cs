// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EdgesView.xaml.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Interaction logic for DependencyItemsView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.View
{
	using System.Windows.Controls;
	using Support;
	using ViewModel;

	/// <summary>
	/// Interaction logic for DependencyItemsView.xaml
	/// </summary>
	[DataContext(typeof(EdgesViewModel))]
	public partial class EdgesView : UserControl
	{
		public EdgesView()
		{
			InitializeComponent();
		}
	}
}
