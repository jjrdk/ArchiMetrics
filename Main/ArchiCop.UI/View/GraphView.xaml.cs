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

namespace ArchiMeter.UI.View
{
	using System.Windows.Controls;

	using ArchiMeter.UI.ViewModel;

	/// <summary>
	/// Interaction logic for GraphView.xaml
	/// </summary>
	[DataContext(typeof(GraphViewModel))]
	public partial class GraphView : UserControl
	{
		public GraphView()
		{
			this.InitializeComponent();
		}
	}
}
