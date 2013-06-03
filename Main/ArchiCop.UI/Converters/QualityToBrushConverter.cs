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

namespace ArchiMeter.UI.Converters
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Windows.Data;
	using System.Windows.Media;

	using ArchiMeter.Common;

	internal class QualityToBrushConverter : IValueConverter
	{
		private readonly IDictionary<CodeQuality, Brush> _brushes = new Dictionary<CodeQuality, Brush>();

		public QualityToBrushConverter()
		{
			this._brushes.Add(CodeQuality.Good, this.GetBrush(Brushes.Green));
			this._brushes.Add(CodeQuality.NeedsReview, this.GetBrush(Brushes.YellowGreen));
			this._brushes.Add(CodeQuality.NeedsRefactoring, this.GetBrush(Brushes.Yellow));
			this._brushes.Add(CodeQuality.NeedsReEngineering, this.GetBrush(Brushes.Orange));
			this._brushes.Add(CodeQuality.Broken, this.GetBrush(Brushes.Red));
			this._brushes.Add(CodeQuality.Incompetent, this.GetBrush(Brushes.HotPink));
		}

		public object Convert(object value, 
							  Type targetType, 
							  object parameter, 
							  CultureInfo culture)
		{
			var quality = (CodeQuality)value;
			return this._brushes[quality];
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
