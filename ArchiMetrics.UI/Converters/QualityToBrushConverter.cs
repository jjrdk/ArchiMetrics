// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualityToBrushConverter.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the QualityToBrushConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.Converters
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Windows.Data;
	using System.Windows.Media;
	using Common;

	internal class QualityToBrushConverter : IValueConverter
	{
		private readonly IDictionary<CodeQuality, Brush> _brushes = new Dictionary<CodeQuality, Brush>();

		public QualityToBrushConverter()
		{
			_brushes.Add(CodeQuality.Good, GetBrush(Brushes.Green));
			_brushes.Add(CodeQuality.NeedsReview, GetBrush(Brushes.YellowGreen));
			_brushes.Add(CodeQuality.NeedsRefactoring, GetBrush(Brushes.Yellow));
			_brushes.Add(CodeQuality.NeedsReEngineering, GetBrush(Brushes.Orange));
			_brushes.Add(CodeQuality.Broken, GetBrush(Brushes.Red));
			_brushes.Add(CodeQuality.Incompetent, GetBrush(Brushes.HotPink));
		}

		public object Convert(object value, 
							  Type targetType, 
							  object parameter, 
							  CultureInfo culture)
		{
			var quality = (CodeQuality)value;
			return _brushes[quality];
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
