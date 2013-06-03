// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularToBrushConverter.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CircularToBrushConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.Converters
{
	using System;
	using System.Globalization;
	using System.Windows.Data;
	using System.Windows.Media;

	internal class CircularToBrushConverter : IValueConverter
	{
		public object Convert(object value, 
		                      Type targetType, 
		                      object parameter, 
		                      CultureInfo culture)
		{
			var isCircular = (bool)value;
			return isCircular ? Brushes.LightPink : Brushes.White;
		}

		public object ConvertBack(object value, 
		                          Type targetType, 
		                          object parameter, 
		                          CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private Brush GetBrush(Brush source)
		{
			var brush = source.Clone();
			brush.Opacity = 0.7;
			brush.Freeze();

			return brush;
		}
	}
}