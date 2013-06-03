// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularReferenceView.xaml.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Interaction logic for CircularReferenceView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.UI.View
{
	using System.Windows.Controls;

	using ArchiMeter.UI.Support;
	using ArchiMeter.UI.ViewModel;

	/// <summary>
	/// Interaction logic for CircularReferenceView.xaml
	/// </summary>
	[DataContext(typeof(CircularReferenceViewModel))]
	public partial class CircularReferenceView : UserControl
	{
		public CircularReferenceView()
		{
			this.InitializeComponent();
		}
	}
}
