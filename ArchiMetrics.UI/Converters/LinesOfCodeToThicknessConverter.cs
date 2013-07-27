// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinesOfCodeToThicknessConverter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LinesOfCodeToThicknessConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.Converters
{
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

	internal class LinesOfCodeToThicknessConverter : IValueConverter
	{
		public object Convert(
			object value,
			Type targetType,
			object parameter,
			CultureInfo culture)
		{
			var linesOfCode = (int)value;
			var length = Math.Log(linesOfCode) * 2;
			return new Thickness(Math.Max(1, length), 0, 0, 0);
		}

		public object ConvertBack(
			object value,
			Type targetType,
			object parameter,
			CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}